using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly SessionRepository _sessions;
        private readonly UserRouteRepository _userRoutes;

        public SessionController(SessionRepository sessions, UserRouteRepository userRoutes)
        {
            _sessions = sessions;
            _userRoutes = userRoutes;
        }

        [HttpGet("{id}")]
        public ActionResult<Session> GetSession([FromRoute] int id)
        {
            Session session = _sessions.GetSessionById(id);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Session>> GetSessions()
        {
            return Ok(_sessions.GetSessions());
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<object>> GetSessionsByUser(int userId)
        {
            var sessions = _sessions.GetSessionsByUser(userId)
                .Select(s => new
                {
                    id = s.ID,
                    userId = s.UserID,
                    routeId = s.RouteID,
                    status = s.Status,
                    loggedAt = s.LoggedAt
                });
            return Ok(sessions);
        }

    }  
}
