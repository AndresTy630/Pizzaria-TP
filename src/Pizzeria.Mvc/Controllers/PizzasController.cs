using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
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

    public async Task<IActionResult> Index(string? buscar, string? orden)
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();

        var items = pizzas.Select(p => new PizzaViewModel
        {
            IdPizza = p.IdPizza,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio
        });

        if (!string.IsNullOrWhiteSpace(buscar))
            items = items.Where(p => p.Nombre.Contains(buscar.Trim(), StringComparison.OrdinalIgnoreCase));

        items = orden switch
        {
            "precio_asc"  => items.OrderBy(p => p.Precio),
            "precio_desc" => items.OrderByDescending(p => p.Precio),
            _             => items.OrderBy(p => p.Nombre)
        };

        return View(new MenuViewModel
        {
            Pizzas = items.ToList(),
            Buscar = buscar,
            Orden = orden ?? "nombre"
        });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Gestion()
    {
        var pizzas = await _pizzaService.VerTodasAsync();

        var model = pizzas.Select(p => new PizzaGestionViewModel
        {
            IdPizza = p.IdPizza,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            Disponible = p.Disponible
        });

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarDisponibilidad(int id)
    {
        try
        {
            await _pizzaService.CambiarDisponibilidadAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Gestion));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var pizza = await _pizzaService.ObtenerPizzaAsync(id);
        if (pizza is null) return NotFound();

        return View(new PizzaEditViewModel
        {
            IdPizza = pizza.IdPizza,
            Nombre = pizza.Nombre,
            Descripcion = pizza.Descripcion,
            Precio = pizza.Precio
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PizzaEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _pizzaService.ActualizarPizzaAsync(
                model.IdPizza, model.Nombre, model.Descripcion ?? string.Empty, model.Precio);

            TempData["Exito"] = "Pizza actualizada.";
            return RedirectToAction(nameof(Gestion));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Crear()
    {
        return View(new PizzaEditViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PizzaEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var pizza = new Pizza(model.Nombre.Trim(), model.Descripcion?.Trim() ?? string.Empty, model.Precio);

            await _pizzaService.RegistrarPizzaAsync(pizza);

            TempData["Exito"] = "Pizza creada.";
            return RedirectToAction(nameof(Gestion));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}