using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Repository;
using Front_Office_Concours_Admin.Services;
using Microsoft.AspNetCore.Mvc;
using StatutCandidature=dotnetProjectShared.Enums.StatutCandidature;

namespace Front_Office_Concours_Admin.Controllers;


public class AnnoncesController : Controller
{
    
    private readonly IAnnonceRepository _repository;
    private readonly ElasticSearchService _elasticService;
    private readonly CandidatureRepository _candidatureRepo;
    private readonly TypeContratRepository _typeContratRepo;
    private readonly TypeEmploiRepository _typeEmploiRepo;

    public AnnoncesController(IAnnonceRepository repository, ElasticSearchService elasticService, CandidatureRepository candidatureRepo,TypeContratRepository typeContratRepo,TypeEmploiRepository typeEmploiRepo)
    {
        _repository = repository;
        _elasticService = elasticService;
        _candidatureRepo = candidatureRepo;
        _typeContratRepo = typeContratRepo;
        _typeEmploiRepo = typeEmploiRepo;
    }

    // GET
    public async Task<IActionResult> Index(
        string? title,
        string? location,
        int? typeContrat,
        int? horaire,
        int currentPage = 1,
        int pageSize = 6)
    {
        var annonces = await _repository.SearchAsync(
            title,
            location,
            typeContrat,
            horaire,
            "dateDesc",
            currentPage,
            pageSize
        );

        ViewBag.TitleFilter = title;
        ViewBag.LocationFilter = location;
        ViewBag.TypeContratFilter = typeContrat;
        ViewBag.HoraireFilter = horaire;

        ViewBag.allTypeContrat = _typeContratRepo.GetAll();
        ViewBag.allTypeEmploi = _typeEmploiRepo.GetAll();

        return View(annonces); 
    }


    
    public IActionResult Details(int id)
    {
        int? candidatId = HttpContext.Session.GetInt32("CandidatId");
        if (candidatId == null)
            return RedirectToAction("Login", "Auth");

        bool isAlreadyApply = _repository.CheckIfUserAlreadyApply(candidatId.Value, id);
        ViewBag.IsAlreadyApply = isAlreadyApply;

        var details = _repository.GetDetailsAnnonceById(id);
        var taches = details?.TachesPrincipales?.Split(',') ?? Array.Empty<string>();

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
            StatutCandidatureId = (int)StatutCandidature.Envoyee,
            StatutCandidature = StatutCandidature.Envoyee,
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