using FMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    public class SQLController : Controller
    {

        /// <summary>
        /// Contexte de la base de données pour interagir avec les entités.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructeur qui initialise le contrôleur avec le contexte de base de données.
        /// </summary>
        /// <param name="context">Contexte de la base de données.</param>
        public SQLController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Action permettant d'ajouter un utilisateur à la base de données via une requête POST.
        /// </summary>
        /// <param name="newUser">Les données de l'utilisateur à ajouter.</param>
        /// <returns>Une réponse HTTP indiquant le succès ou l'échec de l'opération.</returns>
        [HttpPost]
        public IActionResult AddUser([FromBody] UserModel newUser)
        {
            if (newUser == null || string.IsNullOrEmpty(newUser.username) || string.IsNullOrEmpty(newUser.password))
            {
                return BadRequest(new { Message = "Données utilisateur manquantes." });
            }

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { Message = "Utilisateur ajouté avec succès !" });
        }
    }
}
