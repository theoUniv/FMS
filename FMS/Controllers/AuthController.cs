using FMS.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

    // Action pour enregistrer un nouvel utilisateur
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { Message = "Nom d'utilisateur ou mot de passe manquant." });
        }

        // Vérifiez si le nom d'utilisateur existe déjà
        if (_context.Users.Any(u => u.username == request.Username))
        {
            return Conflict(new { Message = "Le nom d'utilisateur existe déjà." });
        }

        // Chiffrement du mot de passe
        var encryptedPassword = _aesCipherService.EncryptPassword(request.Password);  // Chiffrement du mot de passe

        // Créer un nouvel utilisateur
        var newUser = new UserModel
        {
            username = request.Username,
            password = encryptedPassword  // Stocker le mot de passe chiffré
        };

        // Sauvegarder l'utilisateur dans la base de données
        _context.Users.Add(newUser);
        _context.SaveChanges();

        return Ok(new { Message = "Utilisateur enregistré avec succès." });
    }

    // Action pour la connexion de l'utilisateur
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { Message = "Nom d'utilisateur ou mot de passe manquant." });
        }

        // Trouver l'utilisateur dans la base de données
        var user = _context.Users.SingleOrDefault(u => u.username == request.Username);

        if (user == null)
        {
            return Unauthorized(new { Message = "Nom d'utilisateur ou mot de passe incorrect." });
        }

        // Vérifier le mot de passe
        bool isPasswordValid = _aesCipherService.VerifyPassword(user.password, request.Password);  // Vérification du mot de passe

        if (!isPasswordValid)
        {
            return Unauthorized(new { Message = "Nom d'utilisateur ou mot de passe incorrect." });
        }

        // Si l'utilisateur est authentifié, générer un jeton JWT
        var token = _jwtService.GenerateToken(user.username);

        return Ok(new { Token = token });
    }
}
