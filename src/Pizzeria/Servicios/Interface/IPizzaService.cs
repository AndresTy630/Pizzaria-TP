using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Servicios.Interface;

public interface IPizzaService
{
    Task<IEnumerable<Pizza>> VerDisponiblesAsync();
    Task<Pizza?> ObtenerPizzaAsync(int id);
    Task<int> RegistrarPizzaAsync(Pizza pisha);
    Task<IEnumerable<Pizza>> VerTodasAsync();
    Task ActualizarPizzaAsync(int id, string nombre, string descripcion, decimal precio);
    Task CambiarDisponibilidadAsync(int id);
}
