using Front_Office_Concours_Admin.Repository;
using Front_Office_Concours_Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class AnnoncesController : Controller
{
    
    private readonly IAnnonceRepository _repository;
    private readonly ElasticSearchService _elasticService;

    public AnnoncesController(IAnnonceRepository repository, ElasticSearchService elasticService)
    {
        _repository = repository;
        _elasticService = elasticService;
    }

    // GET
    public IActionResult Index(
        string title, string location, string typeContrat, string diplome, string horaire,
        int currentPage = 1, int pageSize = 6)
    {
        
        Console.WriteLine("title=" + title);
        Console.WriteLine("location=" + location);
        Console.WriteLine("typeContrat=" + typeContrat);
        Console.WriteLine("diplome=" + diplome);
        Console.WriteLine("horaire=" + horaire);
        // 1️⃣ Rechercher les IDs correspondants dans Elasticsearch
        var ids = _elasticService.SearchAnnonceIds(title, location, typeContrat, diplome, horaire, currentPage, pageSize);
        Console.WriteLine("IDs Elasticsearch: " + string.Join(",", ids));

        var vm = _repository.GetPagedAnnoncesByIds(ids, currentPage, pageSize);

        Console.WriteLine("Annonces SQL: " + vm.Annonces.Count());
        return View(vm);
    }

    
    public IActionResult Details(int id)
    { 
        return View();
    }

    public IActionResult Apply(int id)
    {
        return View();
    }
}