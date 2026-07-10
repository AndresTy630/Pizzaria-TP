using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IPedidoRepository
    {
        Task<int> CrearPedidoAsync(Pedido pedido);
        Task<Pedido?> ObtenerPorIdAsync(int id);
        Task ActualizarEstadoAsync(int idPedido, EstadoPedido nuevoEstado);
    }
}
