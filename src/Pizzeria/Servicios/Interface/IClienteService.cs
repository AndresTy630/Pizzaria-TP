using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Servicios.Interface;

public interface IClienteService
{
    Task<int> RegistrarClienteAsync(Cliente cliente);
    Task<Cliente?> ObtenerClienteAsync(int id);
}
