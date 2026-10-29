using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public AccountController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    //LOGIN

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

        // Datos que quedan guardados en la sesión (cookie)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        TempData["Exito"] = $"¡Hola, {usuario.Nombre}!";
        return RedirectToAction("Index", "Dashboard");
    }

    //REGISTRO

    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            NombreUsuario = model.Usuario.Trim(),
            Nombre = model.Nombre.Trim(),
            Apellido = model.Apellido.Trim(),
            Email = model.Email.Trim(),
            PasswordHash = model.Password,   // texto plano: el servicio lo hashea
            Telefono = model.Telefono.Trim(),
            Direccion = model.Direccion?.Trim() ?? string.Empty
        };

        try
        {
            await _usuarioService.RegistrarUsuarioAsync(usuario);

            TempData["Exito"] = "Cuenta creada correctamente. Ahora podés iniciar sesión.";
            return RedirectToAction("Login");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    //LOGOUT

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}