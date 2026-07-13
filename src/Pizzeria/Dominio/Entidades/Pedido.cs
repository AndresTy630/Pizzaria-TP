using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Dominio.Entidades;

public class Pedido
{
    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public DateTime FechaHora { get; set; }
    public EstadoPedido Estado { get; private set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public decimal Total { get; private set; }


    private List<DetallePedido> _detalles = new();
    public IReadOnlyCollection<DetallePedido> Detalles => _detalles.AsReadOnly();


    public Pedido()
    {
        Estado = EstadoPedido.EsperaDeConfirmacion;
        FechaHora = DateTime.Now;
    }

    public void CalcularTotal() =>
        /*    if (Detalles == null || !Detalles.Any())
Total = 0;
else
Total = Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);*/

        Total = (Detalles is null || Detalles.Count == 0) ?
            0 : Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

    public void AgregarDetalle(DetallePedido detalle)
    {
        if (detalle == null)
            throw new ArgumentNullException(nameof(detalle));

        _detalles.Add(detalle);
        CalcularTotal();
    }

    public void IniciarPreparacion()
    {
        if (Estado != EstadoPedido.EsperaDeConfirmacion)
            throw new InvalidOperationException("Solo se pueden preparar pedidos que estén en espera.");

        Estado = EstadoPedido.EnPreparacion;
    }

    public void MarcarComoListo()
    {
        if (Estado != EstadoPedido.EnPreparacion)
            throw new InvalidOperationException("El pedido debe estar en preparación para marcarse como listo.");

        Estado = EstadoPedido.Listo;
    }

    public void Enviar()
    {
        if (Estado != EstadoPedido.Listo)
            throw new InvalidOperationException("El pedido debe estar listo para enviarse.");

        Estado = EstadoPedido.EnViaje;
    }

    public void Entregar()
    {
        if (Estado != EstadoPedido.EnViaje)
            throw new InvalidOperationException("El pedido debe estar en viaje para ser entregado.");

        Estado = EstadoPedido.Entregado;
    }
}

