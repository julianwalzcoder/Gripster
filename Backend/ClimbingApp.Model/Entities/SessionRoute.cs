namespace ClimbingApp.Model.Entities;

public class SessionRoute
{
    public int SessionID { get; set; }
    public int RouteID { get; set; }
    public int Tries { get; set; }
    // Rating entfernt, da nach UserRoute verlagert
}
