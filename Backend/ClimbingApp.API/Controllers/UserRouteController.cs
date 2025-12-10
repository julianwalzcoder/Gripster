using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ClimbingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserRouteController : ControllerBase
    {
        protected UserRouteRepository Repository { get; }
        protected UserRepository UserRepository { get; }
        protected ClimbRepository ClimbRepository { get; }
        protected UserSessionRouteRepository _userSessionRouteRepository { get; }

        public UserRouteController(UserRouteRepository repository, UserRepository userRepository, ClimbRepository climbRepository, UserSessionRouteRepository userSessionRouteRepository)
        {
            Repository = repository;
            UserRepository = userRepository;
            ClimbRepository = climbRepository;
            _userSessionRouteRepository = userSessionRouteRepository;
        }

        [HttpGet("{userId}/{routeId}")]
        public ActionResult<UserRoute> GetUserRoute([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserRoute userRoute = Repository.GetUserRouteById(userId, routeId);
            if (userRoute == null)
            {
                return NotFound();
            }
            return Ok(userRoute);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserRoute>> GetUserRoutes()
        {
            return Ok(Repository.GetUserRoutes());
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<UserRoute>> GetUserRoutesByUserId([FromRoute] int userId)
        {
            return Ok(Repository.GetUserRoutesByUserId(userId));
        }

        [HttpGet("route/{routeId}")]
        public ActionResult<IEnumerable<UserRoute>> GetUserRoutesByRouteId([FromRoute] int routeId)
        {
            return Ok(Repository.GetUserRoutesByRouteId(routeId));
        }

        [HttpPost]
        public ActionResult Post([FromBody] UserRoute userRoute)
        {
            if (userRoute == null)
            {
                return BadRequest("UserRoute info not correct");
            }
            
            // Validate that User exists
            var user = UserRepository.GetUserById(userRoute.UserID);
            if (user == null)
            {
                return BadRequest($"User with ID {userRoute.UserID} does not exist");
            }
            
            // Validate that Route exists
            var route = ClimbRepository.GetRouteById(userRoute.RouteID);
            if (route == null)
            {
                return BadRequest($"Route with ID {userRoute.RouteID} does not exist");
            }
            
            bool status = Repository.InsertUserRoute(userRoute);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateUserRoute([FromBody] UserRoute userRoute)
        {
            if (userRoute == null)
            {
                return BadRequest("UserRoute info not correct");
            }
            
            UserRoute existingUserRoute = Repository.GetUserRouteById(userRoute.UserID, userRoute.RouteID);
            if (existingUserRoute == null)
            {
                return NotFound($"UserRoute with UserID {userRoute.UserID} and RouteID {userRoute.RouteID} not found");
            }
            
            bool status = Repository.UpdateUserRoute(userRoute);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }
        
        [HttpPost("{userId}/{routeId}/status/{status}")]
        public ActionResult InsertUserRouteByID(int userId, int routeId, string status)
        {
            var user = UserRepository.GetUserById(userId);
            if (user == null) return BadRequest($"User with ID {userId} does not exist");
            var route = ClimbRepository.GetRouteById(routeId);
            if (route == null) return BadRequest($"Route with ID {routeId} does not exist");

            var ok = Repository.InsertUserRouteByID(userId, routeId, status);
            if (!ok) return BadRequest("Something went wrong");

            // also log the action
            _userSessionRouteRepository.Insert(userId, routeId, status);

            return Ok(new { message = "UserRoute updated and session logged" });
        }

        [HttpPost("{userId}/{routeId}/rating")]
        public ActionResult SetRating([FromRoute] int userId, [FromRoute] int routeId, [FromBody] int? rating)
        {
            if (rating is not null && (rating < 1 || rating > 5))
                return BadRequest("Rating must be 1..5 or null.");
            var ok = Repository.UpsertRating(userId, routeId, rating);
            return ok ? Ok() : BadRequest("Failed to set rating");
        }

        [HttpGet("{userId}/{routeId}/rating")]
        public ActionResult<int?> GetRating([FromRoute] int userId, [FromRoute] int routeId)
        {
            var ur = Repository.GetUserRouteById(userId, routeId);
            if (ur == null) return Ok((int?)null); // Return null instead of 404
            return Ok(ur.Rating);
        }

        [HttpDelete("{userId}/{routeId}")]
        public ActionResult DeleteUserRoute([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserRoute existingUserRoute = Repository.GetUserRouteById(userId, routeId);
            if (existingUserRoute == null)
            {
                return NotFound($"UserRoute with UserID {userId} and RouteID {routeId} not found");
            }
            bool status = Repository.DeleteUserRoute(userId, routeId);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete UserRoute with UserID {userId} and RouteID {routeId}");
        }
    }
}
