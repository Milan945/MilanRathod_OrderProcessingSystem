using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ORS.Service.Contracts;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>  
    /// Registers a new user.  
    /// </summary>  
    /// <param name="email">The email of the user to register.</param>  
    /// <param name="password">The password for the user.</param>  
    /// <returns>A success message if registration is successful.</returns>  
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Email and password cannot be empty.");
        }

        try
        {
            await _userService.AddUserAsync(request.Email, request.Password);
            return Ok("User registered successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  
    /// Authenticates a user by verifying their email and password.  
    /// </summary>  
    /// <param name="email">The email of the user to authenticate.</param>  
    /// <param name="password">The password for the user.</param>  
    /// <returns>A success message if authentication is successful.</returns>  
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Email and password cannot be empty.");
        }

        var isAuthenticated = await _userService.AuthenticateUserAsync(request.Email, request.Password);

        if (isAuthenticated)
        {
            return Ok("Login successful!");
        }
        else
        {
            return Unauthorized("Invalid email or password.");
        }
    }
}