using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class CandidatureController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Details(int id)
    {
        return View();
    }
}