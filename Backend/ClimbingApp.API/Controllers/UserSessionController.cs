using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserSessionController : ControllerBase
    {
        protected UserSessionRepository Repository { get; }
        protected UserRepository UserRepository { get; }
        protected ClimbRepository ClimbRepository { get; }

        public UserSessionController(UserSessionRepository repository, UserRepository userRepository, ClimbRepository climbRepository)
        {
            Repository = repository;
            UserRepository = userRepository;
            ClimbRepository = climbRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserSession>> GetUserSessions()
        {
            return Ok(Repository.GetUserSessions());
        }

        [HttpGet("{userId}/{routeId}")]
        public ActionResult<UserSession> GetUserSessionById([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserSession userSession = Repository.GetUserSessionById(userId, routeId);
            if (userSession == null)
            {
                return NotFound();
            }
            return Ok(userSession);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<UserSession>> GetUserSessionsByUserId([FromRoute] int userId)
        {
            return Ok(Repository.GetUserSessionsByUserId(userId));
        }

        [HttpGet("{userId}/{routeId}/details")]
        public ActionResult<IEnumerable<UserSession>> GetUserSessionsByUserIdAndRouteId([FromRoute] int userId, [FromRoute] int routeId)
        {
            return Ok(Repository.GetUserSessionsByUserIdAndRouteId(userId, routeId));
        }

        [HttpPost]
        public ActionResult Post([FromBody] UserSession userSession)
        {
            if (userSession == null)
            {
                return BadRequest("UserSession info not correct");
            }
            
            if (userSession.UserID.HasValue)
            {
                var user = UserRepository.GetUserById(userSession.UserID.Value);
                if (user == null)
                {
                    return BadRequest($"User with ID {userSession.UserID} does not exist");
                }
            }
            
            var route = ClimbRepository.GetRouteById(userSession.RouteID);
            if (route == null)
            {
                return BadRequest($"Route with ID {userSession.RouteID} does not exist");
            }
            
            bool status = Repository.InsertUserSession(userSession);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Failed to insert user session");
        }

        [HttpPut("{userId}/{routeId}")]
        public ActionResult UpdateUserSession([FromRoute] int userId, [FromRoute] int routeId, [FromBody] UserSession userSession)
        {
            if (userSession == null)
            {
                return BadRequest("UserSession info not correct");
            }
            
            UserSession existingUserSession = Repository.GetUserSessionById(userId, routeId);
            if (existingUserSession == null)
            {
                return NotFound($"UserSession with UserID {userId} and RouteID {routeId} not found");
            }
            
            userSession.UserID = userId;
            userSession.RouteID = routeId;
            bool status = Repository.UpdateUserSession(userSession);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{userId}/{routeId}")]
        public ActionResult DeleteUserSession([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserSession existingUserSession = Repository.GetUserSessionById(userId, routeId);
            if (existingUserSession == null)
            {
                return NotFound($"UserSession with UserID {userId} and RouteID {routeId} not found");
            }
            bool status = Repository.DeleteUserSession(userId, routeId);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete UserSession with UserID {userId} and RouteID {routeId}");
        }
    }
}

