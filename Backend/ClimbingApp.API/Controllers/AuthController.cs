using ClimbingApp.Model.Repositories;
using ClimbingApp.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(AuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _repo.ValidateUser(req.Username, req.Password);
            if (user == null)
            { return Unauthorized("Invalid username or password"); }

            var token = GenerateToken(user);
            return Ok(new { token, username = user.Username, role = user.Role });
        }

        private string GenerateToken(User user)
        {
            var key = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key,
            SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
new Claim(ClaimTypes.Name, user .Username),
new Claim(ClaimTypes.Role, user .Role)
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
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
