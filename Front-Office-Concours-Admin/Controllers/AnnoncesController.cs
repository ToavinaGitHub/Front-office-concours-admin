using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Repository;
using Front_Office_Concours_Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class AnnoncesController : Controller
{
    
    private readonly IAnnonceRepository _repository;
    private readonly ElasticSearchService _elasticService;
    private readonly CandidatureRepository _candidatureRepo;

    public AnnoncesController(IAnnonceRepository repository, ElasticSearchService elasticService, CandidatureRepository candidatureRepo)
    {
        _repository = repository;
        _elasticService = elasticService;
        _candidatureRepo = candidatureRepo;
    }

    // GET
    public IActionResult Index(
        string title, string location, string typeContrat, string diplome, string horaire,
        int currentPage = 1, int pageSize = 6)
    {
        var vm = _repository.GetPagedAnnonces(currentPage, pageSize);

        // Tu peux encore mettre tes filtres dans ViewBag si tu veux
        ViewBag.TitleFilter = title;
        ViewBag.LocationFilter = location;
        ViewBag.TypeContratFilter = typeContrat;
        ViewBag.HoraireFilter = horaire;

        // ⚠ Passe le viewmodel complet et non juste la liste
        return View(vm);
    }


    
    public IActionResult Details(int id)
    { 
        int? candidatId = HttpContext.Session.GetInt32("CandidatId");
        if (candidatId == null)
            return RedirectToAction("Login", "Auth");
        var details = _repository.GetDetailsAnnonceById(id);
        var taches = details?.TachesPrincipales?.Split(',') ?? new string[] { };
        return View(details);
    }

    public IActionResult Apply(int id)
    {
        return View();
    }
    
     [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCandidature(
        int AnnonceId,
        List<DetailsCandidature> DetailsCandidature,
        IFormFile CVFile,
        IFormFile LMFile)
    {
        int? candidatId = HttpContext.Session.GetInt32("CandidatId");
        if (candidatId == null)
            return RedirectToAction("Login", "Auth");

        var candidature = new Candidature
        {
            AnnonceId = AnnonceId,
            CandidatId = candidatId.Value,
            StatutCandidatureId = 1,
            DateCreation = DateTime.Now,
            DetailsCandidature = DetailsCandidature ?? new()
        };

        if (CVFile != null && CVFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await CVFile.CopyToAsync(ms);
            candidature.CV = Convert.ToBase64String(ms.ToArray());
        }

        if (LMFile != null && LMFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await LMFile.CopyToAsync(ms);
            candidature.LettreMotivation = Convert.ToBase64String(ms.ToArray());
        }

        foreach (var d in candidature.DetailsCandidature)
        {
            if (d.PieceFile != null && d.PieceFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await d.PieceFile.CopyToAsync(ms);
                d.PieceJustificative = Convert.ToBase64String(ms.ToArray());
            }
        }

        try
        {
            int newId = _candidatureRepo.CreateCandidature(candidature);
            TempData["SuccessMessage"] = "Candidature envoyée avec succès !";

            // Ici on peut fermer le modal si succès
            return RedirectToAction("Details", "Candidature", new { id = newId });
        }
        catch (ApplicationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.ErrorMessage = ex.Message;

            var annonce = _repository.GetDetailsAnnonceById(AnnonceId);
            ViewBag.OpenModal = true; // ⚡ important pour rouvrir le modal
            return View("~/Views/Annonces/Details.cshtml", annonce);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Une erreur technique est survenue.");
            ViewBag.ErrorMessage = "Une erreur technique est survenue.";

            var annonce = _repository.GetDetailsAnnonceById(AnnonceId);
            ViewBag.OpenModal = true;
            return View("~/Views/Annonces/Details.cshtml", annonce);
        }

    }


}