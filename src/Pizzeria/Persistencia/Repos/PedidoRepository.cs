using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repositorios;

public class PedidoRepository : RepoBase, IPedidoRepository
{
    public PedidoRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CrearPedidoAsync(Pedido pedido)
    {
        using var connection = Connection;

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string sqlPedido = @"
                INSERT INTO Pedido
                (idUsuario, idSucursal, idRepartidor, fechaHora, estado, tipoEntrega, direccionEntrega, total)
                VALUES
                (@IdUsuario, @IdSucursal, @IdRepartidor, @FechaHora, @Estado, @TipoEntrega, @DireccionEntrega, @Total);
                SELECT LAST_INSERT_ID();";

            var idPedido = await connection.ExecuteScalarAsync<int>(
                sqlPedido,
                new
                {
                    pedido.IdUsuario,
                    pedido.IdSucursal,
                    pedido.IdRepartidor,
                    pedido.FechaHora,
                    Estado = (int)pedido.Estado,
                    TipoEntrega = (int)pedido.TipoEntrega,
                    pedido.DireccionEntrega,
                    pedido.Total
                },
                transaction
            );

            const string sqlDetalle = @"
                INSERT INTO DetallePedido
                (idPedido, idPizza, cantidad, precioUnitario)
                VALUES
                (@IdPedido, @IdPizza, @Cantidad, @PrecioUnitario);";

            foreach (var detalle in pedido.Detalles)
            {
                await connection.ExecuteAsync(
                    sqlDetalle,
                    new
                    {
                        IdPedido = idPedido,
                        detalle.IdPizza,
                        detalle.Cantidad,
                        detalle.PrecioUnitario
                    },
                    transaction
                );
            }

            transaction.Commit();

            return idPedido;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id)
    {
        using var connection = Connection;

        const string sqlPedido = @"
            SELECT *
            FROM Pedido
            WHERE idPedido = @Id;";

        var pedido = await connection.QueryFirstOrDefaultAsync<Pedido>( sqlPedido, new { Id = id } );

        if (pedido == null)
            return null;

        const string sqlDetalles = @"
            SELECT *
            FROM DetallePedido
            WHERE idPedido = @IdPedido;";

        var detalles = await connection.QueryAsync<DetallePedido>( sqlDetalles, new { IdPedido = id } );

        foreach (var detalle in detalles)
        {
            pedido.AgregarDetalle(detalle);
        }

        return pedido;
    }

    public async Task ActualizarEstadoAsync( int idPedido, EstadoPedido nuevoEstado)
    {
        using var connection = Connection;

        const string sql = @"
            UPDATE Pedido
            SET estado = @Estado
            WHERE idPedido = @IdPedido;";

        await connection.ExecuteAsync( sql, new { IdPedido = idPedido, Estado = (int)nuevoEstado} );
    }
}