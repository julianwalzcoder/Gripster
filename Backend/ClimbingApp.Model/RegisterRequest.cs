namespace ClimbingApp.Model
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }  // plain text from client
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public int Postcode { get; set; }
        public string City { get; set; }
    }
}
