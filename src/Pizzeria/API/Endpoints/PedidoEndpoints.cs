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

        grupo.MapPost("/", async (CrearPedidoDto request, IPedidoService pedidoService, IPizzaRepository pizzaRepo, IPromocionRepository promoRepo) =>
        {
            try
            {
                var nuevoPedido = new Pedido
                {
                    IdUsuario = request.IdUsuario,
                    IdSucursal = request.IdSucursal,
                    TipoEntrega = request.TipoEntrega,
                    DireccionEntrega = request.DireccionEntrega
                };

                foreach (var item in request.Detalles)
                {
                    var pizza = await pizzaRepo.ObtenerPorIdAsync(item.IdPizza);

                    if (pizza == null || !pizza.Disponible)
                        return Results.BadRequest(new { Error = $"La pizza con ID {item.IdPizza} no existe o no está disponible." });

                    var promos = await promoRepo.ObtenerVigentesPorPizzaAsync(item.IdPizza);

                    var mejor = promos
                        .Select(p => new { Promo = p, Descuento = p.CalcularDescuento(pizza.Precio, item.Cantidad) })
                        .Where(x => x.Descuento > 0)
                        .OrderByDescending(x => x.Descuento)
                        .FirstOrDefault();

                    nuevoPedido.AgregarDetalle(new DetallePedido
                    {
                        IdPizza = item.IdPizza,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = pizza.Precio,
                        Descuento = mejor?.Descuento ?? 0,
                        IdPromocion = mejor?.Promo.IdPromocion
                    });
                }

                var id = await pedidoService.CrearPedidoAsync(nuevoPedido);
                return Results.Created($"/api/pedidos/{id}", new { IdPedido = id, Mensaje = "Pedido recibido, comenzando preparación."});

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