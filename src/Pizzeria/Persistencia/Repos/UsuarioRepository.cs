using Dapper;
using Microsoft.Extensions.Configuration;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Persistencia.Repos;

public class UsuarioRepository : RepoBase, IUsuarioRepository
{
    public UsuarioRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CrearUsuarioAsync(Usuario user)
    {
        const string sql = @"
                INSERT INTO Usuario (nombre, apellido, userName, clave, telefono, direccion, rol) 
                VALUES (@Nombre, @Apellido, @UserName, @Clave, @Telefono, @Direccion, @Rol);
                SELECT LAST_INSERT_ID();";

        using var db = Connection;

        return await db.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        const string sql = "SELECT * FROM Usuario WHERE idUsuario = @Id;";

        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }
}
