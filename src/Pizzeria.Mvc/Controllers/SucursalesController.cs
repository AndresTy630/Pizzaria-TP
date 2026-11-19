using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class SucursalesController : Controller
{
    private readonly ISucursalService _service;

    public SucursalesController(ISucursalService service) => _service = service;

    [AllowAnonymous]
    public async Task<IActionResult> Index() => View(await _service.VerActivasAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Crear() => View(new SucursalCrearViewModel());

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SucursalCrearViewModel model)
    {
        double? lat = double.TryParse(model.Latitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var a) ? a : null;
        double? lng = double.TryParse(model.Longitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var b) ? b : null;

        if (!ModelState.IsValid) return View(model);

        try
        {
            await _service.RegistrarAsync(new Sucursal
                { Direccion = model.Direccion, Telefono = model.Telefono ?? "", Latitud = lat, Longitud = lng });
            TempData["Exito"] = "Sucursal agregada.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}