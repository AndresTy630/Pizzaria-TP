using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.API.Endpoints;

public static class PizzaEndpoints
{
    public static void MapPizzaEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/pizzas").WithTags("Menú de Pizzas");

        grupo.MapGet("/", async (IPizzaService pizzaService) =>
        {
            var menu = await pizzaService.VerDisponiblesAsync();
            return Results.Ok(menu);
        });

        grupo.MapGet("/{id}", async (int id, IPizzaService pizzaService) =>
        {
            var pizza = await pizzaService.ObtenerPizzaAsync(id);
            return pizza is not null ? Results.Ok(pizza) : Results.NotFound("Pizza no encontrada.");
        });
    }
}