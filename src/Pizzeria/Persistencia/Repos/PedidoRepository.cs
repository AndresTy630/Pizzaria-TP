using Dapper;
using Pizzeria.Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repos
{
    public class PedidoRepository : RepoBase, IPedidoRepository
    {
        public PedidoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CrearPedidoAsync(Pedido pedido)
        {
            const string sqlPedido = @"
                INSERT INTO Pedido (idCliente, fechaHora, estado, direccionEntrega, total) 
                VALUES (@IdCliente, @FechaHora, @Estado, @DireccionEntrega, @Total);
                SELECT LAST_INSERT_ID();";

            const string sqlDetalle = @"
                INSERT INTO DetallePedido (idPedido, idPizza, cantidad, precioUnitario) 
                VALUES (@IdPedido, @IdPizza, @Cantidad, @PrecioUnitario);";

            using var db = Connection;

            await db.OpenAsync();
            using var transaction = await db.BeginTransactionAsync();

            try
            {
                var idGenerado = await db.ExecuteScalarAsync<int>(sqlPedido, pedido, transaction);

                foreach (var detalle in pedido.Detalles)
                {
                    detalle.IdPedido = idGenerado;
                }

                await db.ExecuteAsync(sqlDetalle, pedido.Detalles, transaction);

                await transaction.CommitAsync();
                return idGenerado;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Pedido?> ObtenerPorIdAsync(int id)
        {
            const string sql = "SELECT * FROM Pedido WHERE idPedido = @Id;";

            const string sqlDetalles = "SELECT * FROM DetallePedido WHERE idPedido = @Id;";

            using var db = Connection;

            var pedido = await db.QueryFirstOrDefaultAsync<Pedido>(sql, new { Id = id });

            if (pedido != null)
            {
                var detalles = await db.QueryAsync<DetallePedido>(sqlDetalles, new { Id = id });

                foreach (var detalle in detalles)
                {
                    pedido.AgregarDetalle(detalle);
                }
            }

            return pedido;
        }

        public async Task ActualizarEstadoAsync(int idPedido, EstadoPedido nuevoEstado)
        {
            const string sql = "UPDATE Pedido SET estado = @Estado WHERE idPedido = @Id;";

            using var db = Connection;
            await db.ExecuteAsync(sql, new { Estado = (int)nuevoEstado, Id = idPedido });
        }
    }
}
