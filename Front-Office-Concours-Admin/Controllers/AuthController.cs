using Front_Office_Concours_Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Front_Office_Concours_Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly CandidatRepository _repository;

        public AuthController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _repository = new CandidatRepository(connectionString);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(string Email, string MotDePasse)
        {
            var candidat = _repository.GetCandidatByEmail(Email);
            if (candidat == null)
            {
                ViewBag.Error = "Email ou mot de passe incorrect";
                return View();
            }

            var passwordHasher = new PasswordHasher<Candidat>();
            var result = passwordHasher.VerifyHashedPassword(candidat, candidat.MotDePasse, MotDePasse);

            if (result == PasswordVerificationResult.Success)
            {
                // Stocker les informations en session
                HttpContext.Session.SetInt32("CandidatId", candidat.Id);
                HttpContext.Session.SetString("CandidatNom", candidat.Nom);
                HttpContext.Session.SetString("CandidatPrenom", candidat.Prenom);

                return RedirectToAction("Index", "Annonces");
            }
            else
            {
                ViewBag.Error = "Email ou mot de passe incorrect";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("CandidatId");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Signin");
        }
    }
}
