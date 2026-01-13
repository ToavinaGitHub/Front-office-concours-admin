using Microsoft.AspNetCore.Mvc;
using Front_Office_Concours_Admin.Models;
using Front_Office_Concours_Admin.Repository;

namespace Front_Office_Concours_Admin.Controllers
{
    public class CandidatureController : Controller
    {
        private readonly CandidatureRepository _candidatureRepo;
        private readonly TypeContratRepository _typeContratRepo;

        public CandidatureController(CandidatureRepository candidatureRepo, TypeContratRepository typeContratRepo)
        {
            _candidatureRepo = candidatureRepo;
            _typeContratRepo = typeContratRepo;
        }

        // GET: /Candidature/Index
        public IActionResult Index(string keyword = "", string typeContrat = "", int pageNumber = 1, int pageSize = 1)
        {
            int? candidatId = HttpContext.Session.GetInt32("CandidatId");
            if (candidatId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Récupérer candidatures avec recherche + filtre + pagination
            var (candidatures, totalCount) = _candidatureRepo.GetCandidatures(
                candidatId.Value, keyword, typeContrat, pageNumber, pageSize
            );

            ViewData["Keyword"] = keyword;
            ViewData["TypeContrat"] = typeContrat;
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalCount"] = totalCount;
            
            var typeContrats = _typeContratRepo.GetAll();
            ViewBag.TypeContrats = typeContrats;

            return View(candidatures);
        }

        // GET: /Candidature/Details/5
        public IActionResult Details(int id)
        {
            int? candidatId = HttpContext.Session.GetInt32("CandidatId");
            if (candidatId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var candidature = _candidatureRepo.GetCandidatureById(id);

            if (candidature == null || candidature.CandidatId != candidatId.Value)
            {
                return NotFound();
            }

            return View(candidature);
        }
    }
}