using ClimbingApp.Model.Repositories;
using ClimbingApp.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using ClimbingApp.Model;
using Microsoft.AspNetCore.Authorization;

namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthRepository _repo;
        private readonly UserRepository _userRepo;
        private readonly IConfiguration _config;
        public AuthController(AuthRepository repo, UserRepository userRepo, IConfiguration config)
        {
            _repo = repo;
            _userRepo = userRepo;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _repo.ValidateUser(req.Username, req.Password);
            if (user == null)
            { return Unauthorized("Invalid username or password"); }

            var token = GenerateToken(user);
            // Debug: check if user has ID
            Console.WriteLine($"User ID after validation: {user.Id}");
            return Ok(new { token, username = user.Username, role = user.Role, userId = user.Id });
        }

        //register endpoint
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required");

            var user = new User(0)
            {
                Name = req.Name,
                Username = req.Username,
                Mail = req.Mail,
                Street = req.Street,
                StreetNumber = req.StreetNumber,
                Postcode = req.Postcode,
                City = req.City,
                Role = "user" // enforce normal user
            };

            var success = _userRepo.InsertUser(user, req.Password); // uses crypt() insert
            if (!success)
                return BadRequest("Could not create user");

            // Need to retrieve the user to get the generated ID
            var createdUser = _repo.ValidateUser(req.Username, req.Password);
            if (createdUser == null)
                return BadRequest("User created but could not retrieve");

            var token = GenerateToken(createdUser);
            Console.WriteLine($"User ID after registration: {createdUser.Id}");
            return Ok(new { token, username = createdUser.Username, role = createdUser.Role, userId = createdUser.Id });
        }
        private string GenerateToken(User user)
        {
            var key = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key,
            SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };
            var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
