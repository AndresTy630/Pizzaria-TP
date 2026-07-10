using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Persistencia;

namespace Pizzeria.Persistencia.Repos;

public class ClienteRepository : RepoBase, IClienteRepository
{
    public ClienteRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CrearClienteAsync(Cliente cliente)
    {
        const string sql = @"
                INSERT INTO Cliente (nombre, telefono, direccion) 
                VALUES (@Nombre, @Telefono, @Direccion);
                SELECT LAST_INSERT_ID();";

        using var db = Connection;

        return await db.ExecuteScalarAsync<int>(sql, cliente);
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        const string sql = "SELECT * FROM Cliente WHERE idCliente = @Id;";

        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
    }
}
