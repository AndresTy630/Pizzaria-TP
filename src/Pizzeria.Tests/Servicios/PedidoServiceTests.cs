using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Service;

namespace Pizzeria.Tests.Servicios;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IPizzaRepository> _pizzas = new();
    private readonly Mock<ISucursalRepository> _sucursales = new();
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        // PedidoService lanza una simulación en segundo plano que pide un scope:
        // lo mockeamos para que no falle (los delays son de 5 a 12 s, el test ya terminó).
        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(IPedidoRepository))).Returns(_pedidos.Object);

        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(provider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

        _service = new PedidoService(
            _pedidos.Object, _pizzas.Object, _sucursales.Object,
            scopeFactory.Object, Mock.Of<ILogger<PedidoService>>());
    }

    private void SucursalConRepartidores(int repartidores = 2) =>
        _sucursales.Setup(s => s.ObtenerRepartoAsync()).ReturnsAsync(new[]
        {
            new SucursalReparto { IdSucursal = 1, Direccion = "Av. Siempre Viva 123", Repartidores = repartidores, Libres = repartidores }
        });

    private void PizzaDisponible(int id = 1, decimal precio = 100m) =>
        _pizzas.Setup(p => p.ObtenerPorIdAsync(id))
               .ReturnsAsync(new Pizza("Muzza", "Clásica", precio) { IdPizza = id });

    // ---------- Reglas al armar el pedido ----------

    [Fact]
    public async Task Crear_DeliveryEnSucursalSinRepartidores_LanzaExcepcion()
    {
        SucursalConRepartidores(0);
        PizzaDisponible();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.Delivery, "Calle 1", new[] { new LineaPedido(1, 1) }));

        Assert.Contains("repartidores", ex.Message);
        _pedidos.Verify(p => p.CrearPedidoAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task Crear_DeliveryEnSucursalInexistente_LanzaExcepcion()
    {
        SucursalConRepartidores();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 999, TipoEntrega.Delivery, "Calle 1", new[] { new LineaPedido(1, 1) }));
    }

    [Fact]
    public async Task Crear_MasDe50UnidadesDeUnaPizza_LanzaExcepcion()
    {
        PizzaDisponible();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(1, 51) }));

        Assert.Contains("50", ex.Message);
    }

    [Fact]
    public async Task Crear_LineasRepetidasSeSuman_Y_Superan50_LanzaExcepcion()
    {
        PizzaDisponible();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null,
                new[] { new LineaPedido(1, 30), new LineaPedido(1, 30) }));
    }

    [Fact]
    public async Task Crear_PizzaNoDisponible_LanzaExcepcion()
    {
        var agotada = new Pizza("Muzza", "x", 100m) { IdPizza = 1 };
        agotada.MarcarComoAgotada();
        _pizzas.Setup(p => p.ObtenerPorIdAsync(1)).ReturnsAsync(agotada);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(1, 1) }));
    }

    [Fact]
    public async Task Crear_PizzaInexistente_LanzaExcepcion()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(77, 1) }));
    }

    [Fact]
    public async Task Crear_DeliverySinDireccion_LanzaExcepcion()
    {
        SucursalConRepartidores();
        PizzaDisponible();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.Delivery, "   ", new[] { new LineaPedido(1, 1) }));

        Assert.Contains("dirección", ex.Message);
    }

    [Fact]
    public async Task Crear_SinLineas_LanzaExcepcion()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, Array.Empty<LineaPedido>()));

        Assert.Contains("al menos una pizza", ex.Message);
    }

    [Fact]
    public async Task Crear_LineasConCantidadCero_SeIgnoran()
    {
        PizzaDisponible();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(1, 0) }));
    }

    // ---------- Validaciones del pedido ya armado ----------

    [Fact]
    public async Task Crear_PedidoSinUsuario_LanzaExcepcion()
    {
        var pedido = new Pedido { IdUsuario = 0, IdSucursal = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CrearPedidoAsync(pedido));
    }

    [Fact]
    public async Task Crear_PedidoSinSucursal_LanzaExcepcion()
    {
        var pedido = new Pedido { IdUsuario = 1, IdSucursal = 0 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CrearPedidoAsync(pedido));
    }

    // ---------- Camino feliz ----------

    [Fact]
    public async Task Crear_Valido_AgrupaLineasCalculaTotalYGuarda()
    {
        PizzaDisponible(1, 100m);
        _pedidos.Setup(p => p.CrearPedidoAsync(It.IsAny<Pedido>())).ReturnsAsync(7);

        var id = await _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null,
            new[] { new LineaPedido(1, 2), new LineaPedido(1, 1) });

        Assert.Equal(7, id);
        _pedidos.Verify(p => p.CrearPedidoAsync(It.Is<Pedido>(x =>
            x.Total == 300m &&
            x.Detalles.Count == 1 &&
            x.Detalles.Single().Cantidad == 3 &&
            x.Estado == EstadoPedido.EsperaDeConfirmacion)), Times.Once);
    }

    [Fact]
    public async Task Crear_Retiro_IgnoraLaDireccion()
    {
        PizzaDisponible();
        _pedidos.Setup(p => p.CrearPedidoAsync(It.IsAny<Pedido>())).ReturnsAsync(1);

        await _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, "Calle 123", new[] { new LineaPedido(1, 1) });

        _pedidos.Verify(p => p.CrearPedidoAsync(It.Is<Pedido>(x => x.DireccionEntrega == "")), Times.Once);
    }

    // ---------- Resiliencia ----------

    [Fact]
    public async Task Crear_SiLaBaseDeDatosFalla_PropagaLaExcepcion()
    {
        PizzaDisponible();
        _pedidos.Setup(p => p.CrearPedidoAsync(It.IsAny<Pedido>()))
                .ThrowsAsync(new InvalidOperationException("Base caída"));

        // El servicio no la traga: la captura el controller y muestra un mensaje claro
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(1, 1) }));
    }

    [Fact]
    public async Task CambiarEstado_PedidoInexistente_LanzaKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CambiarEstadoPedidoAsync(99, EstadoPedido.Listo));
    }

    [Fact]
    public async Task CambiarEstado_PedidoExistente_ActualizaEnElRepositorio()
    {
        _pedidos.Setup(p => p.ObtenerPorIdAsync(5)).ReturnsAsync(new Pedido { IdPedido = 5 });

        await _service.CambiarEstadoPedidoAsync(5, EstadoPedido.EnPreparacion);

        _pedidos.Verify(p => p.ActualizarEstadoAsync(5, EstadoPedido.EnPreparacion), Times.Once);
    }

    [Fact]
    public async Task Crear_Delivery_AsignaUnRepartidorDeLaSucursal()
    {
        SucursalConRepartidores();
        PizzaDisponible();
        _sucursales.Setup(s => s.ObtenerRepartidoresAsync(1)).ReturnsAsync(new[] { 5, 6 });
        _pedidos.Setup(p => p.CrearPedidoAsync(It.IsAny<Pedido>())).ReturnsAsync(1);

        await _service.CrearPedidoAsync(1, 1, TipoEntrega.Delivery, "Calle 1", new[] { new LineaPedido(1, 1) });

        _pedidos.Verify(p => p.CrearPedidoAsync(
            It.Is<Pedido>(x => x.IdRepartidor == 5 || x.IdRepartidor == 6)), Times.Once);
    }

    [Fact]
    public async Task Crear_Retiro_NoAsignaRepartidor()
    {
        PizzaDisponible();
        _pedidos.Setup(p => p.CrearPedidoAsync(It.IsAny<Pedido>())).ReturnsAsync(1);

        await _service.CrearPedidoAsync(1, 1, TipoEntrega.RetiroEnSucursal, null, new[] { new LineaPedido(1, 1) });

        _pedidos.Verify(p => p.CrearPedidoAsync(It.Is<Pedido>(x => x.IdRepartidor == null)), Times.Once);
    }
}