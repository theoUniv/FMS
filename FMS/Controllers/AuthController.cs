using FMS.Services;
using FMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FMS.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AesCipherService _aesCipherService;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, AesCipherService aesCipherService, JwtService jwtService)
        {
            _context = context;
            _aesCipherService = aesCipherService;
            _jwtService = jwtService;
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
            {
                return BadRequest(new { message = "Nom d'utilisateur ou mot de passe manquant" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.username == user.username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Nom d'utilisateur déjà pris" });
            }

            var encryptedPassword = _aesCipherService.EncryptPassword(user.password);

            var newUser = new UserModel
            {
                username = user.username,
                password = encryptedPassword
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Utilisateur créé avec succès" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
            {
                return BadRequest(new { message = "Nom d'utilisateur ou mot de passe manquant" });
            }

            var storedUser = _context.Users.FirstOrDefault(u => u.username == user.username);
            if (storedUser == null)
            {
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect" });
            }

            var isPasswordValid = _aesCipherService.VerifyPassword(storedUser.password, user.password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect" });
            }

            var token = _jwtService.GenerateToken(user.username);
            return Ok(new { token });
        }

    }
}
