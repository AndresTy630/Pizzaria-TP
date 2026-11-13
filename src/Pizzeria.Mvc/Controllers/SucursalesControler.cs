using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Mvc.Models;

namespace Pizzeria.Mvc.Controllers;

public class SucursalesController : Controller
{
    private readonly ISucursalRepository _repo;

    public SucursalesController(ISucursalRepository repo) => _repo = repo;

    [AllowAnonymous]
    public async Task<IActionResult> Index() => View(await _repo.ObtenerActivasAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Crear() => View(new SucursalCrearViewModel());

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(SucursalCrearViewModel model)
    {
        var okLat = double.TryParse(model.Latitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat);
        var okLng = double.TryParse(model.Longitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lng);

        if (!okLat || !okLng || lat is < -90 or > 90 || lng is < -180 or > 180)
            ModelState.AddModelError(string.Empty, "Marcá la ubicación de la sucursal en el mapa.");

        if (!ModelState.IsValid)
            return View(model);

        await _repo.CrearAsync(new Sucursal
        {
            Direccion = model.Direccion.Trim(),
            Telefono = model.Telefono?.Trim() ?? string.Empty,
            Latitud = lat,
            Longitud = lng
        });

        TempData["Exito"] = "Sucursal agregada.";
        return RedirectToAction(nameof(Index));
    }
}