using Microsoft.AspNetCore.Identity;
using Moq;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Service;

namespace Pizzeria.Tests.Servicios;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repo = new();
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_repo.Object, _hasher);
    }

    private static Usuario Valido() => new()
    {
        NombreUsuario = "juan",
        Nombre = "Juan",
        Apellido = "Pérez",
        Email = "juan@mail.com",
        PasswordHash = "secreto1",   // texto plano: el servicio lo hashea
        Telefono = "1122334455"
    };

    [Theory]
    [InlineData("NombreUsuario")]
    [InlineData("Nombre")]
    [InlineData("Apellido")]
    [InlineData("Email")]
    [InlineData("PasswordHash")]
    [InlineData("Telefono")]
    public async Task Registrar_CampoObligatorioVacio_LanzaExcepcion(string campo)
    {
        var u = Valido();
        typeof(Usuario).GetProperty(campo)!.SetValue(u, "");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.RegistrarUsuarioAsync(u));
        _repo.Verify(r => r.CrearUsuarioAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Registrar_NombreDeUsuarioDuplicado_LanzaExcepcion()
    {
        _repo.Setup(r => r.ObtenerPorUsuarioAsync("juan")).ReturnsAsync(new Usuario());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.RegistrarUsuarioAsync(Valido()));
        Assert.Contains("usuario", ex.Message);
    }

    [Fact]
    public async Task Registrar_EmailDuplicado_LanzaExcepcion()
    {
        _repo.Setup(r => r.ObtenerPorEmailAsync("juan@mail.com")).ReturnsAsync(new Usuario());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.RegistrarUsuarioAsync(Valido()));
        Assert.Contains("email", ex.Message);
    }

    [Fact]
    public async Task Registrar_Valido_GuardaLaContrasenaHasheada()
    {
        Usuario? guardado = null;
        _repo.Setup(r => r.CrearUsuarioAsync(It.IsAny<Usuario>()))
             .Callback<Usuario>(u => guardado = u)
             .ReturnsAsync(5);

        var id = await _service.RegistrarUsuarioAsync(Valido());

        Assert.Equal(5, id);
        Assert.NotNull(guardado);
        Assert.NotEqual("secreto1", guardado!.PasswordHash);
        Assert.NotEqual(PasswordVerificationResult.Failed,
            _hasher.VerifyHashedPassword(guardado, guardado.PasswordHash, "secreto1"));
    }

    private Usuario UsuarioConPassword(string pass)
    {
        var u = Valido();
        u.PasswordHash = _hasher.HashPassword(u, pass);
        return u;
    }

    [Fact]
    public async Task Autenticar_PasswordIncorrecta_DevuelveNull()
    {
        _repo.Setup(r => r.ObtenerPorUsuarioAsync("juan")).ReturnsAsync(UsuarioConPassword("secreto1"));

        Assert.Null(await _service.AutenticarAsync("juan", "otraCosa"));
    }

    [Fact]
    public async Task Autenticar_PasswordCorrecta_DevuelveElUsuario()
    {
        _repo.Setup(r => r.ObtenerPorUsuarioAsync("juan")).ReturnsAsync(UsuarioConPassword("secreto1"));

        var u = await _service.AutenticarAsync("juan", "secreto1");

        Assert.NotNull(u);
        Assert.Equal("juan", u!.NombreUsuario);
    }

    [Fact]
    public async Task Autenticar_ConArroba_BuscaPorEmail()
    {
        _repo.Setup(r => r.ObtenerPorEmailAsync("juan@mail.com")).ReturnsAsync(UsuarioConPassword("secreto1"));

        var u = await _service.AutenticarAsync("juan@mail.com", "secreto1");

        Assert.NotNull(u);
        _repo.Verify(r => r.ObtenerPorUsuarioAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Autenticar_UsuarioInexistente_DevuelveNull()
    {
        Assert.Null(await _service.AutenticarAsync("fantasma", "x"));
    }

    [Theory]
    [InlineData("", "x")]
    [InlineData("juan", "")]
    [InlineData("  ", "  ")]
    public async Task Autenticar_DatosVacios_DevuelveNull(string login, string pass)
    {
        Assert.Null(await _service.AutenticarAsync(login, pass));
    }
}