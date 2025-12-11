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
        private readonly SessionRouteRepository _sessionRoutes;
        private readonly UserRouteRepository _userRoutes;

        public SessionController(SessionRepository sessions, SessionRouteRepository sessionRoutes, UserRouteRepository userRoutes)
        {
            _sessions = sessions;
            _sessionRoutes = sessionRoutes;
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
                .Select(s => new { 
                    id = s.ID, 
                    userId = s.UserID,
                    routeId = s.RouteID, 
                    status = s.Status,
                    loggedAt = s.LoggedAt 
                });
            return Ok(sessions);
        }

        [HttpPost]
        public ActionResult<object> CreateSession([FromBody] CreateSessionRequest req)
        {
            if (req.UserId <= 0 || req.RouteId <= 0) return BadRequest("Invalid payload");
            var id = _sessions.CreateSession(req.UserId, req.RouteId, req.Status, req.LoggedAt);
            if (id <= 0) return BadRequest("Failed to create session");
            return Ok(new { id });
        }

        [HttpPut]
        public ActionResult UpdateSession([FromBody] Session session)
        {
            if (session == null)
            {
                return BadRequest("Session info not correct");
            }
            Session existingSession = _sessions.GetSessionById(session.ID);
            if (existingSession == null)
            {
                return NotFound($"Session with id {session.ID} not found");
            }
            bool status = _sessions.UpdateSession(session);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteSession([FromRoute] int id)
        {
            Session existingSession = _sessions.GetSessionById(id);
            if (existingSession == null)
            {
                return NotFound($"Session with id {id} not found");
            }
            bool status = _sessions.DeleteSession(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete session with id {id}");
        }

        [HttpGet("{sessionId}/routes")]
        public ActionResult<IEnumerable<object>> GetSessionRoutes(int sessionId)
        {
            var routes = _sessionRoutes.GetRoutesForSession(sessionId)
                .Select(r => new { sessionId = r.SessionID, routeId = r.RouteID, tries = r.Tries, status = r.Status });
            return Ok(routes);
        }

        [HttpPost("{sessionId}/routes")]
        public ActionResult UpsertSessionRoute([FromRoute] int sessionId, [FromBody] UpsertSessionRouteRequest req)
        {
            if (req.RouteId <= 0) return BadRequest("RouteId required");
            var ok = _sessionRoutes.UpsertSessionRoute(new SessionRoute
            {
                SessionID = sessionId,
                RouteID = req.RouteId,
                Tries = req.Tries,
                Status = req.Status
            });
            if (!ok) return BadRequest("Failed to upsert session route");

            if (!string.IsNullOrWhiteSpace(req.Status))
            {
                _userRoutes.InsertUserRouteByID(req.UserId, req.RouteId, req.Status);
            }

            return Ok();
        }
    }

    public class CreateSessionRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public string? Status { get; set; }
        public DateTime LoggedAt { get; set; }
    }

    public class SessionRouteItem
    {
        public int sessionId { get; set; }
        public int routeId { get; set; }
        public int? tries { get; set; }
        public string? status { get; set; }
    }

    public class UpsertSessionRouteRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public int? Tries { get; set; }
        public string? Status { get; set; } // Attempted | Top | Flash
    }
}
