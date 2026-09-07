using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPizzaService _pizzaService;

    public HomeController(
    ILogger<HomeController> logger,
    IPizzaService pizzaService)
    {
        _logger = logger;
        _pizzaService = pizzaService;
    }

    public async Task<IActionResult> Index()
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        return View(pizzas);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
