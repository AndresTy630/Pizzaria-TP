using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
            => _clienteRepository = clienteRepository;

    public async Task<int> RegistrarClienteAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ArgumentException("El nombre del cliente es obligatorio.");

        return await _clienteRepository.CrearClienteAsync(cliente);
    }

    public async Task<Cliente?> ObtenerClienteAsync(int id)
            => await _clienteRepository.ObtenerPorIdAsync(id);
}
