using Pizzeria.Dominio.Entidades;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.API.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/clientes").WithTags("Gestión de Clientes");

        grupo.MapPost("/", async (Usuario cliente, IUsuarioService service) =>
        {
            try
            {
                var id = await service.RegistrarUsuarioAsync(cliente);
                return Results.Created($"/api/clientes/{id}", new { IdUsuario = id, Mensaje = "Cliente registrado." });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        grupo.MapGet("/{id:int}", async (int id, IUsuarioService service) =>
        {
            var u = await service.ObtenerUsuarioAsync(id);

            if (u is null)
                return Results.NotFound("Cliente no encontrado.");

            // Se devuelve sin el PasswordHash
            return Results.Ok(new
            {
                u.IdUsuario, u.NombreUsuario, u.Nombre, u.Apellido, u.Email, u.Telefono, u.Direccion
            });
        });
    }
}