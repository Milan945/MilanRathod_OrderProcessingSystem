using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ORS.Data.Contracts;
using ORS.Data.Models;
using ORS.Service.Contracts;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserService : IUserService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly string _hashSalt;

    public UserService(ICustomerRepository customerRepository, IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _hashSalt = configuration["Hashing:HashSalt"];
    }
    public async Task AddUserAsync(string email, string password)
    {
        try
        {
            if (!IsValidEmail(email))
            {
                Log.Warning("Invalid email format: {Email}.", email);
                throw new Exception("Invalid email format.");
            }

            var existingUsers = await _customerRepository.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault(c => c.Email == email);

            if (existingUser != null)
            {
                Log.Warning("User with email {Email} already exists.", email);
                throw new Exception("User with this email already exists.");
            }

            var hashedPassword = HashPassword(password);

            var newUser = new Customer
            {
                Name = email.Split('@')[0],
                Email = email,
                PasswordHash = hashedPassword
            };

            await _customerRepository.AddAsync(newUser);
            await _customerRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error occurred while adding a new user with email {Email}.", email);
            throw;
        }
    }

    public async Task<bool> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            if (!IsValidEmail(email))
            {
                Log.Warning("Invalid email format: {Email}.", email);
                return false;
            }

            var users = await _customerRepository.GetAllAsync();
            var user = users.FirstOrDefault(c => c.Email == email);

            if (user == null)
            {
                Log.Warning("Authentication failed: User with email {Email} not found.", email);
                return false;
            }

            var hashedPassword = HashPassword(password);
            return user.PasswordHash == hashedPassword;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error occurred during authentication for email {Email}.", email);
            throw;
        }
    }

    public string GenerateJwtToken(string username, IList<string> roles)
    {
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

        // Add roles to the claims  
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "YourIssuerHere",
            audience: "YourAudienceHere",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error occurred while validating the email: {Email}.", email);
            throw;
        }
    }

    private string HashPassword(string password)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = $"{password}{_hashSalt}";
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error occurred while hashing the password.");
            throw;
        }
    }
}