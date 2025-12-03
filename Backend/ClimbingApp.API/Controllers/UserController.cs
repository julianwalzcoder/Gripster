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
    public class UserController : ControllerBase
    {
        protected UserRepository Repository { get; }
        public UserController(UserRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            User user = Repository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(Repository.GetUsers());
        }

        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User info not correct");
            }
            bool status = Repository.InsertUser(user);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User info not correct");
            }
            User existingUser = Repository.GetUserById(user.Id);
            if (existingUser == null)
            {
                return NotFound($"User with id {user.Id} not found");
            }
            bool status = Repository.UpdateUser(user);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser([FromRoute] int id)
        {
            User existingUser = Repository.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound($"User with id {id} not found");
            }
            bool status = Repository.DeleteUser(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete user with id {id}");
        }
    }
}
