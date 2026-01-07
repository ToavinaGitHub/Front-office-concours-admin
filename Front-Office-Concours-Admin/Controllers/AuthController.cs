using Microsoft.AspNetCore.Mvc;

namespace Front_Office_Concours_Admin.Controllers;

public class AuthController : Controller
{
    // GET
    public IActionResult Login()
    {
        return View("Login");
    }
    public IActionResult Register()
    {
        return View("Signin");
    }
}