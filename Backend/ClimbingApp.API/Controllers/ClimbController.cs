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

        [HttpPost]
        public ActionResult Post([FromBody] Climb route)
        {
            if (route == null)
            {
                return BadRequest("Climb info not correct");
            }
            bool status = Repository.InsertRoute(route);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Climb route)
        {
            if (route == null)
            {
                return BadRequest("Climb info not correct");
            }
            bool status = Repository.UpdateRoute(route);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            bool status = Repository.DeleteRoute(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest();
        }
    }
}
