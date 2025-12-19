namespace ClimbingApp.Model.Entities;

public class Session
{
    // Backing field to support both Id and ID
    private int _id;

    // Constructor expected by repositories
    public Session(int id)
    {
        _id = id;
    }

    // Parameterless constructor for serializers/ORM
    public Session() { }

    // Both variants to satisfy mixed usage across codebase
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public int ID
    {
        get => _id;
        set => _id = value;
    }

    public int UserID { get; set; }
    public string? CustomName { get; set; }
    public DateTime Date { get; set; }
    public string? Feedback { get; set; }
}
