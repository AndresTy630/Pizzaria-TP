using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pizzeria.Mvc.Controllers;

[Authorize]
public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Dashboard");
    }
}