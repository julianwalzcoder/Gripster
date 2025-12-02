using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRouteController : ControllerBase
    {
        protected UserRouteRepository Repository { get; }
        protected UserRepository UserRepository { get; }
        protected ClimbRepository ClimbRepository { get; }

        public UserRouteController(UserRouteRepository repository, UserRepository userRepository, ClimbRepository climbRepository)
        {
            Repository = repository;
            UserRepository = userRepository;
            ClimbRepository = climbRepository;
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

        [HttpGet("route/{routeId}/rating/average")]
        public ActionResult GetAverageRating([FromRoute] int routeId)
        {
            var avg = Repository.GetAverageRatingForRoute(routeId);
            return Ok(new { routeId, average = avg });
        }

        [HttpGet("{userId}/{routeId}/rating")]
        public ActionResult GetUserRating(int userId, int routeId)
        {
            var ur = Repository.GetUserRouteById(userId, routeId);
            if (ur == null) return NotFound();
            return Ok(new { userId, routeId, rating = ur.Rating });
        }

        [HttpPost]
        public ActionResult Post([FromBody] UserRoute userRoute)
        {
            if (userRoute == null)
            {
                return BadRequest("UserRoute info not correct");
            }

            // Rating validieren (0..5), nur wenn gesetzt
            if (userRoute.Rating.HasValue && (userRoute.Rating < 0 || userRoute.Rating > 5))
            {
                return BadRequest("Rating must be between 0 and 5");
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

            // Rating validieren (0..5), nur wenn gesetzt
            if (userRoute.Rating.HasValue && (userRoute.Rating < 0 || userRoute.Rating > 5))
            {
                return BadRequest("Rating must be between 0 and 5");
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

        [HttpPut("{userId}/{routeId}/rating/{rating}")]
        public ActionResult SetUserRating([FromRoute] int userId, [FromRoute] int routeId, [FromRoute] int rating)
        {
            if (rating < 0 || rating > 5) return BadRequest("Rating must be between 0 and 5.");

            var ur = Repository.GetUserRouteById(userId, routeId);
            if (ur == null)
            {
                // validate foreign keys
                var user = UserRepository.GetUserById(userId);
                if (user == null) return BadRequest($"User with ID {userId} does not exist");
                var route = ClimbRepository.GetRouteById(routeId);
                if (route == null) return BadRequest($"Route with ID {routeId} does not exist");

                // create minimal entry with default status
                var created = Repository.InsertUserRouteByID(userId, routeId, "Unknown");
                if (!created) return BadRequest("Failed to create UserRoute");

                ur = Repository.GetUserRouteById(userId, routeId);
                if (ur == null) return BadRequest("UserRoute could not be read after creation");
            }

            ur.Rating = rating;
            var ok = Repository.UpdateUserRoute(ur);
            return ok ? Ok(new { userId, routeId, rating = ur.Rating }) : BadRequest("Something went wrong");
        }

        [HttpPost("{userId}/{routeId}/{status}")]
        public ActionResult InsertUserRoute([FromRoute] int userId, [FromRoute] int routeId, [FromRoute] string status)
        {
            // Validate that User exists
            var user = UserRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest($"User with ID {userId} does not exist");
            }

            // Validate that Route exists
            var route = ClimbRepository.GetRouteById(routeId);
            if (route == null)
            {
                return BadRequest($"Route with ID {routeId} does not exist");
            }

            // This will insert or update (upsert)
            bool result = Repository.InsertUserRouteByID(userId, routeId, status);
            if (result)
            {
                return Ok(new { message = "UserRoute created or updated successfully" });
            }
            return BadRequest("Something went wrong");
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
