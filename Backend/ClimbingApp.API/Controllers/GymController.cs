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
    public class GymController : ControllerBase
    {
        protected GymRepository Repository { get; }
        public GymController(GymRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Gym> GetGym([FromRoute] int id)
        {
            Gym gym = Repository.GetGymById(id);
            if (gym == null)
            {
                return NotFound();
            }
            return Ok(gym);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Gym>> GetGyms()
        {
            return Ok(Repository.GetGyms());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Gym gym)
        {
            if (gym == null)
            {
                return BadRequest("Gym info not correct");
            }
            bool status = Repository.InsertGym(gym);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateGym([FromBody] Gym gym)
        {
            if (gym == null)
            {
                return BadRequest("Gym info not correct");
            }
            Gym existingGym = Repository.GetGymById(gym.Id);
            if (existingGym == null)
            {
                return NotFound($"Gym with id {gym.Id} not found");
            }
            bool status = Repository.UpdateGym(gym);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteGym([FromRoute] int id)
        {
            Gym existingGym = Repository.GetGymById(id);
            if (existingGym == null)
            {
                return NotFound($"Gym with id {id} not found");
            }
            bool status = Repository.DeleteGym(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete gym with id {id}");
        }
    }
}
