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
            throw new ArgumentException("El apellido del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("El email del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new ArgumentException("La contraseña del cliente es obligatoria.");

        if (string.IsNullOrWhiteSpace(user.Telefono))
            throw new ArgumentException("El teléfono del cliente es obligatorio.");

        return await _usuarioRepository.CrearUsuarioAsync(user);
    }

    public async Task<Usuario?> ObtenerUsuarioAsync(int id)
        => await _usuarioRepository.ObtenerPorIdAsync(id);
}