using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Pizzeria.Persistencia
{
    public abstract class RepoBase
    {
        private readonly string _connectionString;

        protected RepoBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected DbConnection Connection => new MySqlConnection(_connectionString);
    }
}
