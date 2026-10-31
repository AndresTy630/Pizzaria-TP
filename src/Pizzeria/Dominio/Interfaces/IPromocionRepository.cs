using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IPromocionRepository
    {
        Task<IEnumerable<Promocion>> ObtenerVigentesPorPizzaAsync(int idPizza);
        Task<IEnumerable<Promocion>> ObtenerPorPizzaAsync(int idPizza);
        Task<int> CrearAsync(Promocion promocion);
        Task DesactivarAsync(int idPromocion);
        Task<IEnumerable<Promocion>> ObtenerVigentesAsync();
    }
}