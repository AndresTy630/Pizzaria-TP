using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IClienteRepository
    {
        Task<int> CrearClienteAsync(Cliente cliente);
        Task<Cliente?> ObtenerPorIdAsync(int id);
    }
}
