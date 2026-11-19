using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Mvc.Controllers;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Tests.Controllers;

public class PedidosControllerTests
{
    private const int UsuarioLogueado = 1;

    private readonly Mock<IPedidoService> _pedidoService = new();
    private readonly Mock<IPizzaService> _pizzaService = new();
    private readonly Mock<IUsuarioService> _usuarioService = new();
    private readonly Mock<ISucursalService> _sucursales = new();
    private readonly PedidosController _controller;

    public PedidosControllerTests()
    {
        _pizzaService.Setup(s => s.VerDisponiblesAsync()).ReturnsAsync(new List<Pizza>());
        _pizzaService.Setup(s => s.VerTodasAsync()).ReturnsAsync(new List<Pizza>());
        _sucursales.Setup(s => s.VerRepartoAsync()).ReturnsAsync(new List<SucursalReparto>());

        _controller = new PedidosController(
            _pedidoService.Object, _pizzaService.Object, _usuarioService.Object, _sucursales.Object);

        var http = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, UsuarioLogueado.ToString()) }, "test"))
        };
        _controller.ControllerContext = new ControllerContext { HttpContext = http };
        _controller.TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
    }

    private static CrearPedidoViewModel ModeloValido() => new()
    {
        IdSucursal = 3,
        TipoEntrega = TipoEntrega.Delivery,
        DireccionEntrega = "Calle 1",
        Lineas = new() { new LineaPedidoViewModel { IdPizza = 1, Cantidad = 2 } }
    };

    private bool HayError(string key, string texto) =>
        _controller.ModelState[key]!.Errors.Any(e => e.ErrorMessage.Contains(texto));

    private void ServicioLanza(Exception ex) =>
        _pedidoService.Setup(s => s.CrearPedidoAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TipoEntrega>(),
                It.IsAny<string?>(), It.IsAny<IEnumerable<LineaPedido>>()))
            .ThrowsAsync(ex);

    // ---------- Seguridad: cada usuario ve solo lo suyo ----------

    [Fact]
    public async Task Detalle_PedidoDeOtroUsuario_DevuelveNotFound()
    {
        _pedidoService.Setup(s => s.ObtenerPedidoAsync(10))
                      .ReturnsAsync(new Pedido { IdPedido = 10, IdUsuario = 999 });

        Assert.IsType<NotFoundResult>(await _controller.Detalle(10));
    }

    [Fact]
    public async Task Detalle_PedidoInexistente_DevuelveNotFound()
    {
        Assert.IsType<NotFoundResult>(await _controller.Detalle(10));
    }

    [Fact]
    public async Task Estado_PedidoDeOtroUsuario_DevuelveNotFound()
    {
        _pedidoService.Setup(s => s.ObtenerPedidoAsync(10))
                      .ReturnsAsync(new Pedido { IdPedido = 10, IdUsuario = 999 });

        Assert.IsType<NotFoundResult>(await _controller.Estado(10));
    }

    [Fact]
    public async Task Detalle_PedidoPropio_DevuelveLaVista()
    {
        _pedidoService.Setup(s => s.ObtenerPedidoAsync(10))
                      .ReturnsAsync(new Pedido { IdPedido = 10, IdUsuario = UsuarioLogueado });

        var vista = Assert.IsType<ViewResult>(await _controller.Detalle(10));
        Assert.IsType<PedidoDetalleViewModel>(vista.Model);
    }

    // ---------- Crear: validaciones ----------

    [Fact]
    public async Task Crear_SinPizzas_VuelveALaVistaConError_YNoLlamaAlServicio()
    {
        var model = ModeloValido();
        model.Lineas.Clear();

        var result = await _controller.Crear(model);

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError(string.Empty, "al menos una pizza"));
        _pedidoService.Verify(s => s.CrearPedidoAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TipoEntrega>(),
            It.IsAny<string?>(), It.IsAny<IEnumerable<LineaPedido>>()), Times.Never);
    }

    [Fact]
    public async Task Crear_DeliverySinDireccion_MarcaErrorEnElCampo()
    {
        var model = ModeloValido();
        model.DireccionEntrega = "  ";

        var result = await _controller.Crear(model);

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError(nameof(model.DireccionEntrega), "dirección"));
    }

    [Fact]
    public async Task Crear_ModelStateInvalido_VuelveALaVista()
    {
        _controller.ModelState.AddModelError("IdSucursal", "Elegí una sucursal.");

        Assert.IsType<ViewResult>(await _controller.Crear(ModeloValido()));
    }

    // ---------- Crear: resultado ----------

    [Fact]
    public async Task Crear_Valido_RedirigeAlDetalleDelPedidoCreado()
    {
        _pedidoService.Setup(s => s.CrearPedidoAsync(
                UsuarioLogueado, 3, TipoEntrega.Delivery, "Calle 1", It.IsAny<IEnumerable<LineaPedido>>()))
            .ReturnsAsync(9);

        var result = await _controller.Crear(ModeloValido());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Detalle", redirect.ActionName);
        Assert.Equal(9, redirect.RouteValues!["id"]);
    }

    // ---------- Manejo de errores y resiliencia ----------

    [Fact]
    public async Task Crear_ServicioLanzaArgumentException_MuestraElMensajeDeNegocio()
    {
        ServicioLanza(new ArgumentException("La pizza con ID 1 no existe o no está disponible."));

        var result = await _controller.Crear(ModeloValido());

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError(string.Empty, "no existe o no está disponible"));
    }

    [Fact]
    public async Task Crear_CaidaInesperadaDelServicio_MuestraMensajeClaro_SinRomperElFlujo()
    {
        ServicioLanza(new InvalidOperationException("Base de datos caída"));

        var result = await _controller.Crear(ModeloValido());

        Assert.IsType<ViewResult>(result);
        Assert.True(HayError(string.Empty, "No pudimos registrar el pedido"));
        // y no se filtra el mensaje técnico al usuario
        Assert.False(HayError(string.Empty, "Base de datos caída"));
    }
}