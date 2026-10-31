using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repositorios;

public class PromocionRepository : RepoBase, IPromocionRepository
{
    public PromocionRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Promocion>> ObtenerVigentesPorPizzaAsync(int idPizza)
    {
        using var connection = Connection;

        const string sql = @"
            SELECT *
            FROM Promocion
            WHERE idPizza = @IdPizza
              AND activa = TRUE
              AND NOW() BETWEEN fechaInicio AND fechaFin;";

        return await connection.QueryAsync<Promocion>(sql, new { IdPizza = idPizza });
    }
}