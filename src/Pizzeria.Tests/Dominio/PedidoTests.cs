using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Tests.Dominio;

public class PedidoTests
{
    private static DetallePedido Detalle(int cant, decimal precio) =>
        new() { IdPizza = 1, Cantidad = cant, PrecioUnitario = precio };

    [Fact]
    public void NuevoPedido_EmpiezaEnEsperaDeConfirmacion()
    {
        Assert.Equal(EstadoPedido.EsperaDeConfirmacion, new Pedido().Estado);
    }

    [Fact]
    public void AgregarDetalle_CalculaElTotal()
    {
        var p = new Pedido();
        p.AgregarDetalle(Detalle(2, 100m));
        p.AgregarDetalle(Detalle(1, 50m));

        Assert.Equal(250m, p.Total);
    }

    [Fact]
    public void AgregarDetalle_Null_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Pedido().AgregarDetalle(null!));
    }

    [Fact]
    public void FlujoCompleto_Delivery_RecorreTodosLosEstados()
    {
        var p = new Pedido();
        p.IniciarPreparacion();
        Assert.Equal(EstadoPedido.EnPreparacion, p.Estado);
        p.MarcarComoListo();
        Assert.Equal(EstadoPedido.Listo, p.Estado);
        p.Enviar();
        Assert.Equal(EstadoPedido.EnViaje, p.Estado);
        p.Entregar();
        Assert.Equal(EstadoPedido.Entregado, p.Estado);
    }

    [Fact]
    public void Retiro_PuedeEntregarseDesdeListo()
    {
        var p = new Pedido();
        p.IniciarPreparacion();
        p.MarcarComoListo();
        p.Entregar();

        Assert.Equal(EstadoPedido.Entregado, p.Estado);
    }

    [Fact]
    public void Enviar_DesdeEnPreparacion_LanzaExcepcion()
    {
        var p = new Pedido();
        p.IniciarPreparacion();

        Assert.Throws<InvalidOperationException>(() => p.Enviar());
    }

    [Fact]
    public void MarcarComoListo_SinPrepararse_LanzaExcepcion()
    {
        Assert.Throws<InvalidOperationException>(() => new Pedido().MarcarComoListo());
    }

    [Fact]
    public void Entregar_DesdeEsperaDeConfirmacion_LanzaExcepcion()
    {
        Assert.Throws<InvalidOperationException>(() => new Pedido().Entregar());
    }

    [Fact]
    public void IniciarPreparacion_DosVeces_LanzaExcepcion()
    {
        var p = new Pedido();
        p.IniciarPreparacion();

        Assert.Throws<InvalidOperationException>(() => p.IniciarPreparacion());
    }
}