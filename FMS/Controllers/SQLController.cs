using FMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class SQLController : Controller
    {
        private readonly AppDbContext _context;

        // Injection de DbContext
        public SQLController(AppDbContext context)
        {
            _context = context;
        }

        // Action pour ajouter un utilisateur via POST
        [HttpPost]
        public IActionResult AddUser([FromBody] UserModel newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.username) || string.IsNullOrEmpty(newUser.password))
            {
                return BadRequest(new { Message = "Données utilisateur manquantes." });
            }

            // Ajouter l'utilisateur à la base de données
            _context.Users.Add(newUser);  // Utilisation de _context.Users au lieu de _context.Products
            _context.SaveChanges();

            // Retourner une réponse de succès
            return Ok(new { Message = "Utilisateur ajouté avec succès !" });
        }
    }
}
