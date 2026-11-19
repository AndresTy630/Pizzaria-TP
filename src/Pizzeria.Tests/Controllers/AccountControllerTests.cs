using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Mvc.Controllers;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IUsuarioService> _usuarios = new();
    private readonly Mock<IEmpleadoRepository> _empleados = new();
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _controller = new AccountController(_usuarios.Object, _empleados.Object);
        _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
    }

    private static RegistroViewModel Registro() => new()
    {
        Usuario = "juan", Nombre = "Juan", Apellido = "Pérez", Email = "juan@mail.com",
        Password = "secreto1", ConfirmarPassword = "secreto1", Telefono = "1122334455"
    };

    private bool HayError(string texto) =>
        _controller.ModelState[string.Empty]!.Errors.Any(e => e.ErrorMessage.Contains(texto));

    [Fact]
    public async Task Registro_Valido_RedirigeAlLogin()
    {
        var result = await _controller.Registro(Registro());

        Assert.Equal("Login", Assert.IsType<RedirectToActionResult>(result).ActionName);
    }

    [Fact]
    public async Task Registro_UsuarioDuplicado_MuestraElErrorEnLaVista()
    {
        _usuarios.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<Usuario>()))
                 .ThrowsAsync(new ArgumentException("El email ya está registrado."));

        var result = await _controller.Registro(Registro());

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError("ya está registrado"));
    }

    [Fact]
    public async Task Registro_EnviaLaPasswordEnTextoPlano_AlServicio()
    {
        Usuario? recibido = null;
        _usuarios.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<Usuario>()))
                 .Callback<Usuario>(u => recibido = u).ReturnsAsync(1);

        await _controller.Registro(Registro());

        Assert.Equal("secreto1", recibido!.PasswordHash);   // el hasheo lo hace el servicio
    }

    [Fact]
    public async Task Login_CredencialesIncorrectas_VuelveALaVistaConError()
    {
        _usuarios.Setup(s => s.AutenticarAsync("juan", "mala")).ReturnsAsync((Usuario?)null);

        var result = await _controller.Login(new LoginViewModel { Login = "juan", Password = "mala" });

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError("incorrectos"));
    }

    // Este test DESCUBRE una falla del código actual: Registro solo captura ArgumentException.
    // Pasa recién después de agregar el catch (Exception) que está más abajo (punto 5).
    [Fact]
    public async Task Registro_CaidaInesperadaDelServicio_MuestraMensajeClaro()
    {
        _usuarios.Setup(s => s.RegistrarUsuarioAsync(It.IsAny<Usuario>()))
                 .ThrowsAsync(new InvalidOperationException("Base de datos caída"));

        var result = await _controller.Registro(Registro());

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError("No pudimos crear la cuenta"));
    }
}