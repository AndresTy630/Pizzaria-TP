using Microsoft.AspNetCore.Identity;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> RegistrarUsuarioAsync(Usuario user)
    {
        if (string.IsNullOrWhiteSpace(user.NombreUsuario))
            throw new ArgumentException(
                "El nombre de usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Nombre))
            throw new ArgumentException(
                "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Apellido))
            throw new ArgumentException(
                "El apellido es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException(
                "El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new ArgumentException(
                "La contraseña es obligatoria.");

        if (string.IsNullOrWhiteSpace(user.Telefono))
            throw new ArgumentException(
                "El teléfono es obligatorio.");

        var usuarioExistente =
            await _usuarioRepository.ObtenerPorUsuarioAsync(
                user.NombreUsuario);

        if (usuarioExistente != null)
            throw new ArgumentException(
                "El nombre de usuario ya está registrado.");

        var emailExistente =
            await _usuarioRepository.ObtenerPorEmailAsync(
                user.Email);

        if (emailExistente != null)
            throw new ArgumentException(
                "El email ya está registrado.");

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            user.PasswordHash);

        return await _usuarioRepository.CrearUsuarioAsync(user);
    }

    public async Task<Usuario?> ObtenerUsuarioAsync(int id)
    {
        return await _usuarioRepository.ObtenerPorIdAsync(id);
    }

    public async Task<Usuario?> AutenticarAsync(
        string login,
        string password)
    {
        if (string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        Usuario? usuario;

        if (login.Contains('@'))
        {
            usuario = await _usuarioRepository
                .ObtenerPorEmailAsync(login);
        }
        else
        {
            usuario = await _usuarioRepository
                .ObtenerPorUsuarioAsync(login);
        }

        if (usuario == null)
            return null;

        var resultado =
            _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                password);

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        return usuario;
    }
}