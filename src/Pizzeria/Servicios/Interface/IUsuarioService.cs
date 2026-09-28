using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Servicios.Interface;

public interface IUsuarioService
{
    Task<int> RegistrarUsuarioAsync(Usuario user);

    Task<Usuario?> ObtenerUsuarioAsync(int id);

    Task<Usuario?> AutenticarAsync(
        string login,
        string password);
}