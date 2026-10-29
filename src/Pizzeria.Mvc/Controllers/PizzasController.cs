using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

[Authorize]
public class PizzasController : Controller
{
    private readonly IPizzaService _pizzaService;

    public PizzasController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    public async Task<IActionResult> Index()
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        var model = pizzas.Select(p => new PizzaViewModel
        {
            IdPizza = p.IdPizza,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio
        });

        return View(model);
    }
}