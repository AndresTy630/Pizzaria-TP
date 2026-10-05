using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class LandingController : Controller
{
    private readonly IPizzaService _pizzaService;

    public LandingController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    public async Task<IActionResult> Index()
    {
    
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        var model = pizzas.Select(pizza => new PizzaViewModel
        {
            IdPizza = pizza.IdPizza,
            Nombre = pizza.Nombre,
            Descripcion = pizza.Descripcion,
            Precio = pizza.Precio
        });

        return View(model);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}