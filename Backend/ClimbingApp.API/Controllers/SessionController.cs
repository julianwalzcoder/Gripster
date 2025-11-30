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
    public class SessionController : ControllerBase
    {
        protected SessionRepository Repository { get; }
        public SessionController(SessionRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Session> GetSession([FromRoute] int id)
        {
            Session session = Repository.GetSessionById(id);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Session>> GetSessions()
        {
            return Ok(Repository.GetSessions());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Session session)
        {
            if (session == null)
            {
                return BadRequest("Session info not correct");
            }
            bool status = Repository.InsertSession(session);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateSession([FromBody] Session session)
        {
            if (session == null)
            {
                return BadRequest("Session info not correct");
            }
            Session existingSession = Repository.GetSessionById(session.Id);
            if (existingSession == null)
            {
                return NotFound($"Session with id {session.Id} not found");
            }
            bool status = Repository.UpdateSession(session);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteSession([FromRoute] int id)
        {
            Session existingSession = Repository.GetSessionById(id);
            if (existingSession == null)
            {
                return NotFound($"Session with id {id} not found");
            }
            bool status = Repository.DeleteSession(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete session with id {id}");
        }
    }
}
