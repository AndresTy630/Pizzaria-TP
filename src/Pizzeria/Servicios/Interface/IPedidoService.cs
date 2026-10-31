using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Servicios.Interface;

public interface IPedidoService
{
    Task<int> CrearPedidoAsync(Pedido pedido);
    Task<int> CrearPedidoAsync(int idUsuario, int idSucursal, TipoEntrega tipoEntrega, string? direccionEntrega, IEnumerable<LineaPedido> lineas);
    Task<Pedido?> ObtenerPedidoAsync(int id);
    Task CambiarEstadoPedidoAsync(int idPedido, EstadoPedido nuevoEstado);
    Task<IEnumerable<Pedido>> ObtenerPedidosDeUsuarioAsync(int idUsuario);
}