using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repos;

public class PizzaRepository : RepoBase, IPizzaRepository
{
    public PizzaRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Pizza>> ObtenerDisponiblesAsync()
    {
        const string sql = "SELECT * FROM Pizza WHERE disponible = 1;";

        using var db = Connection;

        return await db.QueryAsync<Pizza>(sql);
    }

    public async Task<Pizza?> ObtenerPorIdAsync(int id)
    {
        const string sql = "SELECT * FROM Pizza WHERE idPizza = @Id;";

        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Pizza>(sql, new { Id = id });
    }
}
