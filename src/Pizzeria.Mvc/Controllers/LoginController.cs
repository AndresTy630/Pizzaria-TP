using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class LoginController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public LoginController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _usuarioService.AutenticarAsync(model.Login, model.Password);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "El usuario/email o la contraseña son incorrectos.");
            return View(model);
        }

        TempData["Exito"] = $"¡Hola, {usuario.Nombre}!";
        return RedirectToAction("Index", "Dashboard");
    }
}