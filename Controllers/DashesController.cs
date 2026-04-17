using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using DashBackend.Helpers;
using DashBackend.Models;
using DashBackend.Repositories;

namespace DashBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashesController : ControllerBase
    {
        private readonly ILogger<DashesController> _logger;

        private readonly IDashRepository _dashRepository;

        public DashesController(IDashRepository dashRepository, ILogger<DashesController> logger)
        {
            _dashRepository = dashRepository ?? throw new ArgumentNullException(nameof(dashRepository));
            _logger = logger;
        }

        // GET: api/dashes
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Dash>> GetAll(CancellationToken ct = default)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogDebug("GetAll called");
            return Ok(_dashRepository.GetMyDashes(Guid.Parse(userId)));
        }

        // GET: api/dashes/{id}
        [Authorize]
        [HttpGet("{id:guid}")]
        public ActionResult<Dash> GetById(Guid id, CancellationToken ct = default)
        {
            var dash = _dashRepository.GetById(id);
            if (dash != null)
                return Ok(dash);

            return NotFound();
        }

        // POST: api/dashes
        [HttpPost]
        public ActionResult<Dash> Create([FromBody] CreateDashRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Name is required.");

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dash = new Dash
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                Name = req.Name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _dashRepository.Add(dash);
            _dashRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = dash.Id }, dash);
        }

        // PUT: api/dashes/{id}
        [Authorize]
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] UpdateDashRequest req)
        {
            if (req is null)
                return BadRequest();

            if ((_dashRepository.GetById(id) is Dash existing) == false)
                return NotFound();

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var updated = new Dash
            {
                Id = existing.Id,
                Name = string.IsNullOrWhiteSpace(req.Name) ? existing.Name : req.Name.Trim(),
                UserId = Guid.Parse(userId),
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _dashRepository.Update(updated);
            _dashRepository.SaveChanges();
            return NoContent();
        }

        // DELETE: api/dashes/{id}
        [Authorize]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            if ((_dashRepository.GetById(id) is Dash existing) == false)
                return NotFound();

            _dashRepository.Delete(existing);
            _dashRepository.SaveChanges();
            return NoContent();
        }

        public class CreateDashRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        public class UpdateDashRequest
        {
            public string? Name { get; set; }
        }
    }
}