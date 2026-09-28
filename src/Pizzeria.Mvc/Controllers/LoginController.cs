using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class LoginController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public LoginController(
        IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario =
            await _usuarioService.AutenticarAsync(
                model.Login,
                model.Password);

        if (usuario == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "El usuario/email o la contraseña son incorrectos.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.IdUsuario.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.NombreUsuario),

            new Claim(
                ClaimTypes.Email,
                usuario.Email)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults
                .AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme,
            principal);

        return RedirectToAction(
            "Index",
            "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme);

        return RedirectToAction(
            "Landing",
            "Landing");
    }

    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return Content(
            "No tenés permisos para acceder a esta página.");
    }
}