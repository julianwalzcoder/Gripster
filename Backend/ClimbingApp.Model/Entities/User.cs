namespace ClimbingApp.Model.Entities;

public class User
{
    public User(int id) { Id = id; }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Mail { get; set; }
    public string PasswordHash { get; set; }
    public string Street { get; set; }
    public int StreetNumber { get; set; }
    public int Postcode { get; set; }
    public string City { get; set; }
    public string Role { get; set; }
}