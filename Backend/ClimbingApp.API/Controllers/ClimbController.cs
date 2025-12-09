using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ClimbingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClimbController : ControllerBase
    {
        protected ClimbRepository Repository { get; }

        public ClimbController(ClimbRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Climb> GetClimb([FromRoute] int id)
        {
            Climb route = Repository.GetRouteById(id);
            if (route == null)
            {
                return NotFound();
            }
            return Ok(route);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Climb>> GetClimbs()
        {
            return Ok(Repository.GetRoutes());
        }

        // ADMIN: create climb
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Post([FromBody] Climb route)
        {
            if (route == null)
                return BadRequest("Climb info not correct");

            var status = Repository.InsertRoute(route);
            return status ? Ok() : BadRequest();
        }

        // ADMIN: update climb metadata (grade, dates, etc.)
        [HttpPut]
        [Authorize(Roles = "admin")]
        public ActionResult Put([FromBody] Climb climb)
        {
            if (climb == null)
                return BadRequest("Climb info not correct");

            var existingRoute = Repository.GetRouteById(climb.Id);
            if (existingRoute == null)
                return NotFound($"Route with id {climb.Id} not found");

            var status = Repository.UpdateRoute(climb);
            return status ? Ok() : BadRequest("Something went wrong");
        }

        // ADMIN: delete climb
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            var status = Repository.DeleteRoute(id);
            return status ? NoContent() : BadRequest();
        }
    }
}
