using System.Security.Claims;
[Route("SessionLog")]
[ApiController]
[Authorize]
public class SessionLogController : ControllerBase
{
    private readonly SessionRepository _sessions;
    private readonly SessionRouteRepository _sessionRoutes;

    private int CurrentUserId() =>
        int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")!.Value);

    // List of projects (routes with one or more sessions), grouped by route
    [HttpGet("me/projects")]
    public ActionResult<IEnumerable<object>> MyProjects()
    {
        var uid = CurrentUserId();
        // SELECT SR.RouteID, S.Date, SR.Tries FROM SessionRoute SR JOIN Session S ON S.ID=SR.SessionID WHERE S.UserID=@uid ORDER BY S.Date DESC
        var items = _sessionRoutes.GetByUserGrouped(uid); // returns per-route list of dates/tries
        return Ok(items);
    }
}