using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class AnnoncesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
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