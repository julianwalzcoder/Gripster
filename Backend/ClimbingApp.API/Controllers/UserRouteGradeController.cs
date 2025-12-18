using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ClimbingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserRouteGradeController : ControllerBase
    {
        protected UserRouteGradeRepository Repository { get; }
        protected UserRepository UserRepository { get; }
        protected ClimbRepository ClimbRepository { get; }

        public UserRouteGradeController(UserRouteGradeRepository repository, UserRepository userRepository, ClimbRepository climbRepository)
        {
            Repository = repository;
            UserRepository = userRepository;
            ClimbRepository = climbRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserRouteGrade>> GetUserRouteGrades()
        {
            return Ok(Repository.GetUserRouteGrades());
        }

        [HttpGet("{userId}/{routeId}")]
        public ActionResult<UserRouteGrade> GetUserRouteGradeById([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserRouteGrade userRouteGrade = Repository.GetUserRouteGradeById(userId, routeId);
            if (userRouteGrade == null)
            {
                return NotFound();
            }
            return Ok(userRouteGrade);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<UserRouteGrade>> GetUserRouteGradesByUserId([FromRoute] int userId)
        {
            return Ok(Repository.GetUserRouteGradesByUserId(userId));
        }

        [HttpGet("gym/{gymId}")]
        public ActionResult<IEnumerable<UserRouteGrade>> GetUserRouteGradesByGymId([FromRoute] int gymId)
        {
            return Ok(Repository.GetUserRouteGradesByGymId(gymId));
        }

        [HttpGet("{userId}/{routeId}/details")]
        public ActionResult<IEnumerable<UserRouteGrade>> GetUserRouteGradesByUserIdAndRouteId([FromRoute] int userId, [FromRoute] int routeId)
        {
            return Ok(Repository.GetUserRouteGradesByUserIdAndRouteId(userId, routeId));
        }

        [HttpPost]
        public ActionResult Post([FromBody] UserRouteGrade userRouteGrade)
        {
            if (userRouteGrade == null)
            {
                return BadRequest("UserRouteGrade info not correct");
            }
            
            if (userRouteGrade.UserID.HasValue)
            {
                var user = UserRepository.GetUserById(userRouteGrade.UserID.Value);
                if (user == null)
                {
                    return BadRequest($"User with ID {userRouteGrade.UserID} does not exist");
                }
            }
            
            var route = ClimbRepository.GetRouteById(userRouteGrade.RouteID);
            if (route == null)
            {
                return BadRequest($"Route with ID {userRouteGrade.RouteID} does not exist");
            }
            
            bool status = Repository.InsertUserRouteGrade(userRouteGrade);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Failed to insert user session");
        }

        [HttpPut("{userId}/{routeId}")]
        public ActionResult UpdateUserRouteGrade([FromRoute] int userId, [FromRoute] int routeId, [FromBody] UserRouteGrade userRouteGrade)
        {
            if (userRouteGrade == null)
            {
                return BadRequest("UserRouteGrade info not correct");
            }
            
            UserRouteGrade existingUserRouteGrade = Repository.GetUserRouteGradeById(userId, routeId);
            if (existingUserRouteGrade == null)
            {
                return NotFound($"UserRouteGrade with UserID {userId} and RouteID {routeId} not found");
            }
            
            userRouteGrade.UserID = userId;
            userRouteGrade.RouteID = routeId;
            bool status = Repository.UpdateUserRouteGrade(userRouteGrade);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{userId}/{routeId}")]
        public ActionResult DeleteUserRouteGrade([FromRoute] int userId, [FromRoute] int routeId)
        {
            UserRouteGrade existingUserRouteGrade = Repository.GetUserRouteGradeById(userId, routeId);
            if (existingUserRouteGrade == null)
            {
                return NotFound($"UserRouteGrade with UserID {userId} and RouteID {routeId} not found");
            }
            bool status = Repository.DeleteUserRouteGrade(userId, routeId);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete UserRouteGrade with UserID {userId} and RouteID {routeId}");
        }
    }
}

