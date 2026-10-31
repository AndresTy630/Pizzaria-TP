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

    public async Task<IEnumerable<Promocion>> ObtenerPorPizzaAsync(int idPizza)
    {
        using var connection = Connection;

        const string sql = @"
            SELECT *
            FROM Promocion
            WHERE idPizza = @IdPizza
            ORDER BY fechaInicio DESC;";

        return await connection.QueryAsync<Promocion>(sql, new { IdPizza = idPizza });
    }

    public async Task<int> CrearAsync(Promocion p)
    {
        using var connection = Connection;

        const string sql = @"
            INSERT INTO Promocion
            (idPizza, titulo, tipo, valor, cantidadMinima, fechaInicio, fechaFin, activa)
            VALUES
            (@IdPizza, @Titulo, @Tipo, @Valor, @CantidadMinima, @FechaInicio, @FechaFin, TRUE);
            SELECT LAST_INSERT_ID();";

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            p.IdPizza,
            p.Titulo,
            Tipo = (int)p.Tipo,
            p.Valor,
            p.CantidadMinima,
            p.FechaInicio,
            p.FechaFin
        });
    }

    public async Task DesactivarAsync(int idPromocion)
    {
        using var connection = Connection;

        await connection.ExecuteAsync(
            "UPDATE Promocion SET activa = FALSE WHERE idPromocion = @Id;",
            new { Id = idPromocion });
    }

    public async Task<IEnumerable<Promocion>> ObtenerVigentesAsync()
    {
        using var connection = Connection;

        const string sql = @"
            SELECT *
            FROM Promocion
            WHERE activa = TRUE
            AND NOW() BETWEEN fechaInicio AND fechaFin;";

        return await connection.QueryAsync<Promocion>(sql);
    }
}