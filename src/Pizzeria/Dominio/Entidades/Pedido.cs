using Pizzeria.Dominio.Enums;

namespace Pizzeria.Dominio.Entidades;

public class Pedido
{
    public int IdPedido { get; set; }

    public int IdUsuario { get; set; }

    public int IdSucursal { get; set; }

    public int? IdRepartidor { get; set; }

    public DateTime FechaHora { get; set; }

    public EstadoPedido Estado { get; private set; }

    public TipoEntrega TipoEntrega { get; set; }

    public string DireccionEntrega { get; set; } = string.Empty;

    public decimal Total { get; private set; }

    private List<DetallePedido> _detalles = new();

    public IReadOnlyCollection<DetallePedido> Detalles => _detalles.AsReadOnly();

    public Pedido()
    {
        Estado = EstadoPedido.EsperaDeConfirmacion;
        FechaHora = DateTime.Now;
    }

    public void CalcularTotal()
    {
        Total = (Detalles is null || Detalles.Count == 0)
            ? 0
            : Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
    }

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
            throw new InvalidOperationException(
                "El pedido no puede comenzar a prepararse en su estado actual.");

        Estado = EstadoPedido.EnPreparacion;
    }

    public void MarcarComoListo()
    {
        if (Estado != EstadoPedido.EnPreparacion)
            throw new InvalidOperationException(
                "El pedido no puede marcarse como listo en su estado actual.");

        Estado = EstadoPedido.Listo;
    }

    public void Enviar()
    {
        if (Estado != EstadoPedido.Listo)
            throw new InvalidOperationException(
                "El pedido no puede enviarse en su estado actual.");

        Estado = EstadoPedido.EnViaje;
    }

    public void Entregar()
    {
        if (Estado != EstadoPedido.EnViaje &&
            Estado != EstadoPedido.Listo)
            throw new InvalidOperationException(
                "El pedido no puede marcarse como entregado en su estado actual.");

        Estado = EstadoPedido.Entregado;
    }
}