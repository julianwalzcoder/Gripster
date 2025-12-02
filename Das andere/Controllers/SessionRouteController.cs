using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionRouteController : ControllerBase
    {
        protected SessionRouteRepository Repository { get; }
        public SessionRouteController(SessionRouteRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{sessionId}/{routeId}")]
        public ActionResult<SessionRoute> GetSessionRoute([FromRoute] int sessionId, [FromRoute] int routeId)
        {
            SessionRoute sessionRoute = Repository.GetSessionRouteById(sessionId, routeId);
            if (sessionRoute == null)
            {
                return NotFound();
            }
            return Ok(sessionRoute);
        }

        [HttpGet]
        public ActionResult<IEnumerable<SessionRoute>> GetSessionRoutes()
        {
            return Ok(Repository.GetSessionRoutes());
        }

        [HttpPost]
        public ActionResult Post([FromBody] SessionRoute sessionRoute)
        {
            if (sessionRoute == null)
            {
                return BadRequest("SessionRoute info not correct");
            }
            bool status = Repository.InsertSessionRoute(sessionRoute);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateSessionRoute([FromBody] SessionRoute sessionRoute)
        {
            if (sessionRoute == null)
            {
                return BadRequest("SessionRoute info not correct");
            }
            SessionRoute existingSessionRoute = Repository.GetSessionRouteById(sessionRoute.SessionID, sessionRoute.RouteID);
            if (existingSessionRoute == null)
            {
                return NotFound($"SessionRoute with SessionID {sessionRoute.SessionID} and RouteID {sessionRoute.RouteID} not found");
            }
            bool status = Repository.UpdateSessionRoute(sessionRoute);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{sessionId}/{routeId}")]
        public ActionResult DeleteSessionRoute([FromRoute] int sessionId, [FromRoute] int routeId)
        {
            SessionRoute existingSessionRoute = Repository.GetSessionRouteById(sessionId, routeId);
            if (existingSessionRoute == null)
            {
                return NotFound($"SessionRoute with SessionID {sessionId} and RouteID {routeId} not found");
            }
            bool status = Repository.DeleteSessionRoute(sessionId, routeId);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete SessionRoute with SessionID {sessionId} and RouteID {routeId}");
        }
    }
}
