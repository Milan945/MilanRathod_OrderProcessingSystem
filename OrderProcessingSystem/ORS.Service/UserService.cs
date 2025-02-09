using Microsoft.Extensions.Configuration;
using ORS.Data.Models;
using ORS.Data.Repositories;
using ORS.Service.Contracts;
using System.Security.Cryptography;
using System.Text;

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
        // Check if user already exists  
        var existingUsers = await _customerRepository.GetAllAsync();
        var existingUser = existingUsers.FirstOrDefault(c => c.Email == email);

        if (existingUser != null)
        {
            throw new Exception("User with this email already exists.");
        }

        // Hash the password  
        var hashedPassword = HashPassword(password);

        // Create a new user  
        var newUser = new Customer
        {
            Name = email.Split('@')[0], // Just an example  
            Email = email,
            PasswordHash = hashedPassword
        };

        // Save the user  
        await _customerRepository.AddAsync(newUser);
        await _customerRepository.SaveChangesAsync();
    }

    public async Task<bool> AuthenticateUserAsync(string email, string password)
    {
        var users = await _customerRepository.GetAllAsync();
        var user = users.FirstOrDefault(c => c.Email == email);
        if (user == null)
        {
            return false;
        }

        var hashedPassword = HashPassword(password);

        return user.PasswordHash == hashedPassword;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();

        var saltedPassword = $"{password}{_hashSalt}";

        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}