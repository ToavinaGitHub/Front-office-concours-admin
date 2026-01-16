using Microsoft.AspNetCore.Mvc;
using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Repository;
using Microsoft.AspNetCore.Http;

namespace Front_Office_Concours_Admin.Controllers
{
    public class CandidatureController : Controller
    {
        private readonly CandidatureRepository _candidatureRepo;
        private readonly TypeContratRepository _typeContratRepo;
        private readonly IAnnonceRepository _annonceRepo;

        public CandidatureController(
            CandidatureRepository candidatureRepo,
            TypeContratRepository typeContratRepo,
            IAnnonceRepository annonceRepo)
        {
            _candidatureRepo = candidatureRepo;
            _typeContratRepo = typeContratRepo;
            _annonceRepo = annonceRepo;
        }

        // =========================
        // LISTE DES CANDIDATURES
        // =========================
        public IActionResult Index(string keyword = "", string typeContrat = "", int pageNumber = 1, int pageSize = 5)
        {
            int? candidatId = HttpContext.Session.GetInt32("CandidatId");
            if (candidatId == null)
                return RedirectToAction("Login", "Auth");

            var (candidatures, totalCount) = _candidatureRepo.GetCandidatures(
                candidatId.Value, keyword, typeContrat, pageNumber, pageSize
            );

            ViewData["Keyword"] = keyword;
            ViewData["TypeContrat"] = typeContrat;
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalCount"] = totalCount;

            ViewBag.TypeContrats = _typeContratRepo.GetAll();

            return View(candidatures);
        }
        
        public IActionResult Details(int id)
        {
            int? candidatId = HttpContext.Session.GetInt32("CandidatId");
            if (candidatId == null)
                return RedirectToAction("Login", "Auth");

            DetailsCandidatureResponse details = _annonceRepo.GetDetailsCandidatureById(id);

            if (details == null)
                return NotFound();

            // Vérifie que la candidature appartient au candidat connecté
            if (details.CandidatID != candidatId.Value)
                return RedirectToAction("Login", "Auth");

            foreach (var exigence in details.Exigences)
            {
                Console.WriteLine(exigence.Libelle);   
            }
            return View(details);
        }

    }
}
