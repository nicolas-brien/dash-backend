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
        private readonly IBlockRepository _blockRepository;

        public DashesController(IDashRepository dashRepository, IBlockRepository blockRepository, ILogger<DashesController> logger)
        {
            _dashRepository = dashRepository ?? throw new ArgumentNullException(nameof(dashRepository));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
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
            {
                var blocks = _blockRepository.GetBlocks(dash.Id);

                return Ok(new {
                    dash = new { id = dash.Id, name = dash.Name, settings = new { columns = dash.Columns, rowHeight = dash.RowHeight, displayGrid = dash.DisplayGrid } },
                    blocks = blocks.Select(b => new { i = b.Id.ToString(), b.Text, b.x, y = b.y, w = b.w, h = b.h })
                });
            }

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
                Columns = req.Settings?.Columns ?? existing.Columns,
                RowHeight = req.Settings?.RowHeight ?? existing.RowHeight,
                DisplayGrid = req.Settings?.DisplayGrid ?? existing.DisplayGrid,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _dashRepository.Update(updated);
            _dashRepository.SaveChanges();

            if (req.Blocks != null && req.Blocks.Any())
            {
                foreach (var blockReq in req.Blocks)
                {
                    var block = new Block
                    {
                        Id = blockReq.i,
                        DashId = existing.Id,
                        Text = blockReq.Text,
                        x = blockReq.x,
                        y = blockReq.y,
                        w = blockReq.w,
                        h = blockReq.h
                    };

                    _blockRepository.Upsert(block);
                }
                _blockRepository.SaveChanges();
            }
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

            var blocks = _blockRepository.GetBlocks(id);
            if (blocks.Any())
            {
                foreach (var block in blocks)
                {
                    _blockRepository.Delete(block);
                }
                _blockRepository.SaveChanges();
            }

            return NoContent();
        }

        public class CreateDashRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        public class UpdateDashRequest
        {
            public string? Name { get; set; }

            public IEnumerable<BlockUpdateRequest>? Blocks { get; set; }

            public UpdateDashSettingsRequest? Settings { get; set; }
        }

        public class UpdateDashSettingsRequest
        {
            public int? Columns { get; set; }
            public int? RowHeight { get; set; }
            public bool? DisplayGrid { get; set; }
        }

        public class BlockUpdateRequest
        {
            public Guid i { get; set; }
            public string Text { get; set; } = string.Empty;
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }
    }
}