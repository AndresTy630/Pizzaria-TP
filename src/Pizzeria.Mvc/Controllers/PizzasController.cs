using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Mvc.Controllers;

[Authorize]
public class PizzasController : Controller
{
    private readonly IPizzaService _pizzaService;
    private readonly IPromocionService _promocionService;

    public PizzasController(IPizzaService pizzaService, IPromocionService promocionService)
    {
        _pizzaService = pizzaService;
        _promocionService = promocionService;
    }

    public async Task<IActionResult> Index(string? buscar, string? orden, bool soloOfertas = false)
    {
        var pizzas = await _pizzaService.VerDisponiblesAsync();
        var promos = (await _promocionService.ObtenerVigentesAsync()).ToLookup(p => p.IdPizza);

        var items = pizzas.Select(p =>
        {
            var delPizza = promos[p.IdPizza].ToList();

            // Mejor promo que ya descuenta comprando 1 pizza
            var inmediata = delPizza
                .Select(x => new { Promo = x, Descuento = x.CalcularDescuento(p.Precio, 1) })
                .Where(x => x.Descuento > 0)
                .OrderByDescending(x => x.Descuento)
                .FirstOrDefault();

            // Si no hay, una promo que exige cantidad mínima
            var condicionada = inmediata is null
                ? delPizza.OrderBy(x => x.CantidadMinima).FirstOrDefault()
                : null;

            var promoMostrada = inmediata?.Promo ?? condicionada;

            return new PizzaViewModel
            {
                IdPizza = p.IdPizza,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                PrecioFinal = p.Precio - (inmediata?.Descuento ?? 0),
                OfertaTexto = promoMostrada?.Resumen(),
                OfertaHasta = promoMostrada?.FechaFin
            };
        });

        if (!string.IsNullOrWhiteSpace(buscar))
            items = items.Where(p => p.Nombre.Contains(buscar.Trim(), StringComparison.OrdinalIgnoreCase));

        if (soloOfertas)
            items = items.Where(p => p.TieneOferta);

        items = orden switch
        {
            "precio_asc"  => items.OrderBy(p => p.PrecioFinal),
            "precio_desc" => items.OrderByDescending(p => p.PrecioFinal),
            _             => items.OrderBy(p => p.Nombre)
        };

        return View(new MenuViewModel
        {
            Pizzas = items.ToList(),
            Buscar = buscar,
            Orden = orden ?? "nombre",
            SoloOfertas = soloOfertas
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

    // ---------- DISPONIBILIDAD ----------

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

    // ---------- EDITAR ----------

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

    // ---------- OFERTAS ----------

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Ofertas(int id)
    {
        var model = await ArmarOfertasAsync(id, new PromocionFormViewModel());
        if (model is null) return NotFound();

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearOferta(int id, [Bind(Prefix = "Nueva")] PromocionFormViewModel nueva)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _promocionService.CrearAsync(new Promocion
                {
                    IdPizza = id,
                    Titulo = nueva.Titulo.Trim(),
                    Tipo = nueva.Tipo,
                    Valor = nueva.Valor,
                    CantidadMinima = nueva.CantidadMinima,
                    FechaInicio = nueva.FechaInicio,
                    FechaFin = nueva.FechaFin
                });

                TempData["Exito"] = "Oferta creada.";
                return RedirectToAction(nameof(Ofertas), new { id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        var model = await ArmarOfertasAsync(id, nueva);
        if (model is null) return NotFound();

        return View(nameof(Ofertas), model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DesactivarOferta(int id, int idPromocion)
    {
        await _promocionService.DesactivarAsync(idPromocion);
        TempData["Exito"] = "Oferta desactivada.";
        return RedirectToAction(nameof(Ofertas), new { id });
    }

    private async Task<OfertasViewModel?> ArmarOfertasAsync(int idPizza, PromocionFormViewModel form)
    {
        var pizza = await _pizzaService.ObtenerPizzaAsync(idPizza);
        if (pizza is null) return null;

        var promos = await _promocionService.ObtenerPorPizzaAsync(idPizza);

        return new OfertasViewModel
        {
            IdPizza = pizza.IdPizza,
            Nombre = pizza.Nombre,
            Precio = pizza.Precio,
            Nueva = form,
            Promociones = promos.Select(p =>
            {
                var estado = p.Estado();
                var texto = p.Tipo == TipoPromocion.Porcentaje
                    ? $"{p.Valor:0.##}% off"
                    : $"${p.Valor:N0} menos por pizza";
                if (p.CantidadMinima > 1) texto += $" (desde {p.CantidadMinima} pizzas)";

                return new PromocionItemViewModel
                {
                    IdPromocion = p.IdPromocion,
                    Titulo = p.Titulo,
                    Descripcion = texto,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Estado = estado,
                    PuedeDesactivar = estado is "Vigente" or "Programada"
                };
            }).ToList()
        };
    }
}