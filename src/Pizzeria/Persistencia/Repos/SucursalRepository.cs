using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repositorios;

public class SucursalRepository : RepoBase, ISucursalRepository
{
    public SucursalRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Sucursal>> ObtenerActivasAsync()
    {
        using var connection = Connection;
        return await connection.QueryAsync<Sucursal>("SELECT * FROM Sucursal WHERE activa = TRUE;");
    }

    public async Task<IEnumerable<SucursalReparto>> ObtenerRepartoAsync()
    {
        using var connection = Connection;

        const string sql = @"
            SELECT s.idSucursal AS IdSucursal,
                   s.direccion  AS Direccion,
                   CAST(COUNT(e.idEmpleado) AS SIGNED) AS Repartidores,
                   CAST(COALESCE(SUM(
                       CASE WHEN e.idEmpleado IS NOT NULL
                             AND NOT EXISTS (SELECT 1 FROM Pedido p
                                             WHERE p.idRepartidor = e.idEmpleado AND p.estado = 3)
                            THEN 1 ELSE 0 END), 0) AS SIGNED) AS Libres
            FROM Sucursal s
            LEFT JOIN Empleado e ON e.idSucursal = s.idSucursal AND e.rol = 2
            WHERE s.activa = TRUE
            GROUP BY s.idSucursal, s.direccion;";

        return await connection.QueryAsync<SucursalReparto>(sql);
    }
}