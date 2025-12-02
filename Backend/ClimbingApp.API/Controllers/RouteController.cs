using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClimbingRouteController : ControllerBase
    {
        protected ClimbRepository Repository { get; }
        public ClimbingRouteController(ClimbRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Climb> GetRoute([FromRoute] int id)
        {
            Climb route = Repository.GetRouteById(id);
            if (route == null)
            {
                return NotFound();
            }
            return Ok(route);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Climb>> GetRoutes()
        {
            return Ok(Repository.GetRoutes());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Climb route)
        {
            if (route == null)
            {
                return BadRequest("Route info not correct");
            }
            bool status = Repository.InsertRoute(route);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateRoute([FromBody] Climb route)
        {
            if (route == null)
            {
                return BadRequest("Route info not correct");
            }
            Climb existingRoute = Repository.GetRouteById(route.Id);
            if (existingRoute == null)
            {
                return NotFound($"Route with id {route.Id} not found");
            }
            bool status = Repository.UpdateRoute(route);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRoute([FromRoute] int id)
        {
            Climb existingRoute = Repository.GetRouteById(id);
            if (existingRoute == null)
            {
                return NotFound($"Route with id {id} not found");
            }
            bool status = Repository.DeleteRoute(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete route with id {id}");
        }

        [AllowAnonymous]
        [HttpGet("average-rating/{routeId}")]
        public ActionResult<decimal?> GetAverageRating([FromRoute] int routeId, [FromServices] ClimbRepository climbRepository)
        {
            var avg = climbRepository.GetAverageRatingForRoute(routeId);
            return Ok(avg);
        }
    }
}
