using Front_Office_Concours_Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;
    
public class CandidatController : Controller
{
    private readonly CandidatRepository _repository;

    public CandidatController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        _repository = new CandidatRepository(connectionString);
    }
    
    [HttpGet]
    public IActionResult Inscription()
    {
        return View("~/Views/Auth/Signin.cshtml", new InscriptionViewModel());
    }


    [HttpPost]
    public IActionResult Inscription(
        string Nom,
        string Prenom,
        DateOnly DateNaissance,
        string Adresse,
        string Telephone,
        string Genre,
        string Email,
        string MotDePasse,
        string ConfirmMotDePasse
    )
    {
        if (MotDePasse != ConfirmMotDePasse)
        {
            ViewBag.Error = "Les mots de passe ne correspondent pas";
            return View("~/Views/Auth/Signin.cshtml");
        }

        var passwordHasher = new PasswordHasher<Candidat>();

        var candidat = new Candidat
        {
            Nom = Nom,
            Prenom = Prenom,
            DateNaissance = DateNaissance,
            Adresse = Adresse,
            Telephone = Telephone,
            Genre = Genre,
            Email = Email,
            DateCreation = DateTime.Now
        };

        // Hash du mot de passe
        candidat.MotDePasse = passwordHasher.HashPassword(candidat, MotDePasse);

        _repository.AddCandidat(candidat);

        return RedirectToAction("Login", "Auth");
    }
}
