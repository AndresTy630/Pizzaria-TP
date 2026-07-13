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

        grupo.MapPost("/", async (CrearPedidoDto request, IPedidoService pedidoService, IPizzaRepository pizzaRepo) =>
        {
            try
            {
                var nuevoPedido = new Pedido
                {
                    IdCliente = request.IdCliente,
                    DireccionEntrega = request.DireccionEntrega,
                };

                foreach (var item in request.Detalles)
                {
                    var pizza = await pizzaRepo.ObtenerPorIdAsync(item.IdPizza);
                    if (pizza == null || !pizza.Disponible)
                    {
                        return Results.BadRequest(new { Error = $"La pizza con ID {item.IdPizza} no existe o no esta disponible." });
                    }

                    var detalle = new DetallePedido
                    {
                        IdPizza = item.IdPizza,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = pizza.Precio
                    };

                    nuevoPedido.AgregarDetalle(detalle);
                }

                var id = await pedidoService.CrearPedidoAsync(nuevoPedido);
                return Results.Created($"/api/pedidos/{id}", new { IdPedido = id, Mensaje = "Pedido recibido, comenzando preparacion." });
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

        grupo.MapGet("/{id}", async (int id, IPedidoService pedidoService) =>
        {
            var pedido = await pedidoService.ObtenerPedidoAsync(id);
            return pedido is not null ? Results.Ok(pedido) : Results.NotFound("Pedido no encontrado.");
        });
    }
}