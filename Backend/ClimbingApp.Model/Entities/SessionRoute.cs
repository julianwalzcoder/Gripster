namespace ClimbingApp.Model.Entities;

public class SessionRoute
{
    public int SessionID { get; set; }
    public int RouteID { get; set; }
    public int? Tries { get; set; } // allow null when not tracked
    public string? Status { get; set; } // Attempted | Top | Flash per session
    // Rating entfernt, da nach UserRoute verlagert
}
