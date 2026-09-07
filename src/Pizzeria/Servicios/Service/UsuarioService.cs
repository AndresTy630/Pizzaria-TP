using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
            => _usuarioRepository = usuarioRepository;

    public async Task<int> RegistrarUsuarioAsync(Usuario user)
    {
        if (string.IsNullOrWhiteSpace(user.Nombre))
            throw new ArgumentException("El nombre del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Apellido))
            throw new ArgumentException("El nombre del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Pass))
            throw new ArgumentException("La contraseña del cliente es obligatorio.");
        
        if (string.IsNullOrWhiteSpace(user.Telefono))
            throw new ArgumentException("El telefono del cliente es obligatorio.");

        return await _usuarioRepository.CrearUsuarioAsync(user);
    }

    public async Task<Usuario?> ObtenerUsuarioAsync(int id)
            => await _usuarioRepository.ObtenerPorIdAsync(id);
}
