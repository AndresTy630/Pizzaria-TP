using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repositorios;

public class PizzaRepository : RepoBase, IPizzaRepository
{
    public PizzaRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Pizza>> ObtenerDisponiblesAsync()
    {
        using var connection = Connection;

        const string sql = @"
            SELECT *
            FROM Pizza
            WHERE disponible = TRUE;";

        return await connection.QueryAsync<Pizza>(sql);
    }

    public async Task<Pizza?> ObtenerPorIdAsync(int id)
    {
        using var connection = Connection;

        const string sql = @"
            SELECT *
            FROM Pizza
            WHERE idPizza = @Id;";

        return await connection.QueryFirstOrDefaultAsync<Pizza>( sql, new { Id = id });
    }

    public async Task<int> CrearPizzaAsync(Pizza pisha)
    {
        using var connection = Connection;

        const string sql = @"
            INSERT INTO Pizza
            (nombre, descripcion, precio, disponible)
            VALUES
            (@Nombre, @Descripcion, @Precio, @Disponible);
            SELECT LAST_INSERT_ID();";

        return await connection.ExecuteScalarAsync<int>( sql, pisha );
    }
}