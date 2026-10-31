using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repositorios;

public class EmpleadoRepository : RepoBase, IEmpleadoRepository
{
    public EmpleadoRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<Empleado?> ObtenerPorUsuarioAsync(int idUsuario)
    {
        using var db = Connection;

        const string sql = "SELECT * FROM Empleado WHERE idUsuario = @IdUsuario;";

        return await db.QueryFirstOrDefaultAsync<Empleado>(sql, new { IdUsuario = idUsuario });
    }
}