using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IPizzaRepository
    {
        Task<IEnumerable<Pizza>> ObtenerDisponiblesAsync();
        Task<Pizza?> ObtenerPorIdAsync(int id);
        Task<int> CrearPizzaAsync(Pizza pisha);
    }
}
