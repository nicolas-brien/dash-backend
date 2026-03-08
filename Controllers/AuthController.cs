using Microsoft.AspNetCore.Mvc;

using DashBackend.Helpers;
using DashBackend.Repositories;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly JwtService _jwt;

    public AuthController(IUserRepository users, JwtService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = new
                {
                    usernameOrEmail = "Username or email is required",
                    password = "Password is required"
                }
            });
        }

        var user = _users.GetByUsernameOrEmail(dto.UsernameOrEmail);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        if (!AuthHelper.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = _jwt.GenerateToken(user);

        return Ok(new
        {
            accessToken = token,
            user = new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                fullName = user.FullName,
                role = user.Role
            }
        });
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized(new { message = "Missing or invalid Authorization header" });
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var userId = _jwt.ValidateToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }
        else if (userId == "Expired")
        {
            return Unauthorized(new { message = "Token has expired" });
        }

        var user = _users.GetById(Guid.Parse(userId));
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            fullName = user.FullName,
            role = user.Role
        });
    }
}
