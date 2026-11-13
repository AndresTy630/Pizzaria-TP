using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IPizzaService _pizzaService;
    private readonly ISucursalRepository _sucursalRepository;

    public HomeController(
        IPizzaService pizzaService,
        ISucursalRepository sucursalRepository)
    {
        _pizzaService = pizzaService;
        _sucursalRepository = sucursalRepository;
    }

    public async Task<IActionResult> Index()
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        var pizzasAleatorias = pizzas
            .OrderBy(p => Random.Shared.Next())
            .Take(6)
            .Select(p => new PizzaViewModel
            {
                IdPizza = p.IdPizza,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio
            })
            .ToList();

        return View(pizzasAleatorias);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}