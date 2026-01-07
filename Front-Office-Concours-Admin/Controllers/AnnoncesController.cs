using Front_Office_Concours_Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class AnnoncesController : Controller
{
    
    private readonly IAnnonceRepository _repository;

    public AnnoncesController(IAnnonceRepository repository)
    {
        _repository = repository;
    }

    // GET
    public IActionResult Index(int currentPage = 1, int pageSize = 6)
    {
        Console.WriteLine("AnnoncesController::Index");
        var vm = _repository.GetPagedAnnonces(currentPage, pageSize);
        Console.WriteLine($"CurrentPage: {vm.CurrentPage}, TotalItems: {vm.TotalItems}, Annonces.Count: {vm.Annonces.Count}");
        foreach (var annonce in vm.Annonces)
        {
            Console.WriteLine($"Id: {annonce.Id}, Titre: {annonce.Titre}, Lieu: {annonce.LieuPoste}");
        }

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