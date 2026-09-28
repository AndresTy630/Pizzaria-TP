using Microsoft.AspNetCore.Mvc;

namespace Pizzeria.Mvc.Controllers;

public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Dashboard");
    }
}