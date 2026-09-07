using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<int> CrearUsuarioAsync(Usuario user);
        Task<Usuario?> ObtenerPorIdAsync(int id);
    }
}
