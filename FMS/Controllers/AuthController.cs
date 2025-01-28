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

        /// <summary>
        /// Inscrit un nouvel utilisateur. Le mot de passe est chiffré avant d'être stocké.
        /// </summary>
        /// <param name="user">Modèle utilisateur contenant le nom d'utilisateur et le mot de passe</param>
        /// <returns>Retourne une réponse indiquant si l'inscription a réussi ou échoué</returns>
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

        /// <summary>
        /// Authentifie un utilisateur. Si le nom d'utilisateur et le mot de passe sont valides, génère un token JWT.
        /// </summary>
        /// <param name="user">Modèle utilisateur contenant le nom d'utilisateur et le mot de passe</param>
        /// <returns>Retourne un token JWT si l'utilisateur est authentifié avec succès, sinon une erreur</returns>
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
