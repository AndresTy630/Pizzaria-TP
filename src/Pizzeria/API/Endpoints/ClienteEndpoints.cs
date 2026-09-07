using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.API.Endpoints;
public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/clientes").WithTags("Gestión de Clientes");

        grupo.MapPost("/", async (Usuario cliente, IUsuarioService clienteService) =>
        {
            try
            {
                var id = await clienteService.RegistrarUsuarioAsync(cliente);
                return Results.Created($"/api/clientes/{id}", new { IdCliente = id, Mensaje = "Cliente registrado." });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        grupo.MapGet("/{id}", async (int id, IUsuarioService clienteService) =>
        {
            var cliente = await clienteService.ObtenerUsuarioAsync(id);
            return cliente is not null ? Results.Ok(cliente) : Results.NotFound("Cliente no encontrado.");
        });
    }
}