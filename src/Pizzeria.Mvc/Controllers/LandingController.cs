using Microsoft.AspNetCore.Mvc;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class LandingController : Controller
{
    private readonly IPizzaService _pizzaService;

    public LandingController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    public async Task<IActionResult> Landing()
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        return View("~/Views/Pages/Landing/Landing.cshtml", pizzas);
    }
}