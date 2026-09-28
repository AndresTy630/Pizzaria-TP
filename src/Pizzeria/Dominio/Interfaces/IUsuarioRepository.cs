using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<int> CrearUsuarioAsync(Usuario user);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorUsuarioAsync(string usuario);
    }
}
