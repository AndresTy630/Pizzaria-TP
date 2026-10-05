using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class RegistroController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public RegistroController(
        IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            NombreUsuario = model.Usuario.Trim(),

            Nombre = model.Nombre.Trim(),

            Apellido = model.Apellido.Trim(),

            Email = model.Email.Trim(),

            // Acá todavía está en texto plano
            // porque el servicio lo va a hashear.
            PasswordHash = model.Password,

            Telefono = model.Telefono.Trim(),

            Direccion =
                model.Direccion?.Trim()
                ?? string.Empty
        };

        try
        {
            await _usuarioService
                .RegistrarUsuarioAsync(usuario);

            TempData["RegistroExitoso"] =
                "Cuenta creada correctamente. " +
                "Ahora podés iniciar sesión.";

            return RedirectToAction(
                "Login",
                "Login");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model);
        }
    }
}