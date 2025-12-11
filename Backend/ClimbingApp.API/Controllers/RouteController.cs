using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace ClimbingApp.API.Controllers
{
    [Route("api/route")]
    [Route("api/ClimbingRoute")] // keep for backward compatibility
    [ApiController]
    [Authorize]
    public class ClimbingRouteController : ControllerBase
    {
        protected ClimbRepository Repository { get; }
        private readonly AdminRepository _admins;

        public ClimbingRouteController(ClimbRepository repository, AdminRepository admins)
        {
            Repository = repository;
            _admins = admins;
        }

        public sealed class AddClimbDto
        {
            public int GymId { get; set; }
            public int GradeId { get; set; }
            public string? SetDate { get; set; }     // ISO string (e.g., 2025-12-11T00:00:00Z)
            public string? RemoveDate { get; set; }  // ISO string or null
            public string? Status { get; set; }      // optional; remove if not in entity
        }

        [HttpGet("{id:int}")]
        public ActionResult<Climb> GetRoute([FromRoute] int id)
        {
            var route = Repository.GetRouteById(id);
            if (route == null) return NotFound();
            return Ok(route);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Climb>> GetRoutes()
        {
            return Ok(Repository.GetRoutes());
        }

        [HttpPut("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Climb body)
        {
            if (body == null) return BadRequest("Body is empty");
            if (id <= 0 || (body.Id != 0 && body.Id != id)) return BadRequest("Id mismatch");

            // enforce admin
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(idClaim)) return Unauthorized();
            var userId = int.Parse(idClaim);
            var admin = _admins.GetByUserId(userId);
            if (admin == null) return StatusCode(StatusCodes.Status403Forbidden, "User is not an admin");

            // ensure non-null SetDate
            if (body.SetDate == default) body.SetDate = DateTime.UtcNow;
            body.Id = id;
            body.AdminID = admin.Id;

            var ok = Repository.UpdateRoute(body);
            return ok ? Ok() : BadRequest("Failed to update route");
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteRoute([FromRoute] int id)
        {
            var existingRoute = Repository.GetRouteById(id);
            if (existingRoute == null) return NotFound($"Route with id {id} not found");

            var status = Repository.DeleteRoute(id);
            return status ? NoContent() : BadRequest($"Unable to delete route with id {id}");
        }

        [AllowAnonymous]
        [HttpGet("average-rating/{routeId:int}")]
        public ActionResult<decimal?> GetAverageRating([FromRoute] int routeId, [FromServices] ClimbRepository climbRepository)
        {
            var avg = climbRepository.GetAverageRatingForRoute(routeId);
            return Ok(avg);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddClimbDto dto)
        {
            if (dto == null) return BadRequest("Invalid body");

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(idClaim)) return Unauthorized();

            var userId = int.Parse(idClaim);
            var admin = _admins.GetByUserId(userId);
            if (admin == null)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not an admin");
                // or: return Forbid(); // no message, lets middleware handle it

            DateTime? setDate = null, removeDate = null;
            if (!string.IsNullOrWhiteSpace(dto.SetDate) &&
                DateTime.TryParse(dto.SetDate, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var sd))
                setDate = sd;

            if (!string.IsNullOrWhiteSpace(dto.RemoveDate) &&
                DateTime.TryParse(dto.RemoveDate, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var rd))
                removeDate = rd;

            var entity = new Climb(0)
            {
                GymID = dto.GymId,
                GradeID = dto.GradeId,
                SetDate = setDate ?? DateTime.UtcNow,
                RemoveDate = removeDate,
                AdminID = admin.Id
            };

            var ok = Repository.InsertRoute(entity);
            return ok ? Ok() : BadRequest("Failed to insert route");
        }
    }
}
