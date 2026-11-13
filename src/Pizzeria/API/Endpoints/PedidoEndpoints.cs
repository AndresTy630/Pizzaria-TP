using Pizzeria.API.DTO;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.API.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/pedidos").WithTags("Gestión de Pedidos");

        grupo.MapPost("/", async (CrearPedidoDto request, IPedidoService pedidoService) =>
        {
            try
            {
                var lineas = request.Detalles.Select(d => new LineaPedido(d.IdPizza, d.Cantidad));

                var id = await pedidoService.CrearPedidoAsync(
                    request.IdUsuario, request.IdSucursal, request.TipoEntrega,
                    request.DireccionEntrega, lineas);

                return Results.Created($"/api/pedidos/{id}",
                    new { IdPedido = id, Mensaje = "Pedido recibido, comenzando preparación." });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return Results.Problem("Ocurrió un error interno al procesar el pedido.");
            }
        });

        grupo.MapGet("/{id}", async ( int id, IPedidoService pedidoService) =>
        {
            var pedido = await pedidoService.ObtenerPedidoAsync(id);

            return pedido is not null ? Results.Ok(pedido) : Results.NotFound("Pedido no encontrado."); 
        });
    }
}