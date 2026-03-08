using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DashBackend.Helpers;
using DashBackend.Models;
using DashBackend.Repositories;

namespace DashBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger;
        }

        // GET: api/users
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll(CancellationToken ct = default)
        {
            _logger.LogDebug("GetAll called");
            return Ok(_userRepository.GetAll());
        }

        // GET: api/users/{id}
        [Authorize]
        [HttpGet("{id:guid}")]
        public ActionResult<User> GetById(Guid id, CancellationToken ct = default)
        {
            var user = _userRepository.GetById(id);
            if (user != null)
                return Ok(user);

            return NotFound();
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> Create([FromBody] CreateUserRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.FullName) || string.IsNullOrWhiteSpace(req.Email))
                return BadRequest("Name and Email are required.");
            var user = new User
            {
                Id = Guid.NewGuid(),
                NetworkId = new Guid("e14bf9e4-597d-4eb7-aa28-a05804551b51"),
                Username = req.UserName.Trim(),
                Email = req.Email.Trim(),
                FullName = req.FullName.Trim(),
                PasswordHash = AuthHelper.HashPassword(req.Password),
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(user);
            _userRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [Authorize]
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateUserRequest req)
        {
            if (req is null)
                return BadRequest();

            if (req.Role is not null && req.Role != "user" && req.Role != "networkAdmin" && req.Role != "admin")
                return BadRequest("Invalid role specified.");

            if ((_userRepository.GetById(id) is User existing) == false)
                return NotFound();

            var updated = new User
            {
                Id = existing.Id,
                NetworkId = existing.NetworkId,
                Username = existing.Username,
                PasswordHash = existing.PasswordHash,
                FullName = string.IsNullOrWhiteSpace(req.FullName) ? existing.FullName : req.FullName.Trim(),
                Email = string.IsNullOrWhiteSpace(req.Email) ? existing.Email : req.Email.Trim(),
                Role = string.IsNullOrWhiteSpace(req.Role) ? existing.Role : req.Role.Trim(),
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _userRepository.Update(updated);
            _userRepository.SaveChanges();
            return NoContent();
        }

        // DELETE: api/users/{id}
        [Authorize]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            if ((_userRepository.GetById(id) is User existing) == false)
                return NotFound();

            _userRepository.Delete(existing);
            _userRepository.SaveChanges();
            return NoContent();
        }

        public class CreateUserRequest
        {
            public string UserName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class UpdateUserRequest
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
        }
    }
}