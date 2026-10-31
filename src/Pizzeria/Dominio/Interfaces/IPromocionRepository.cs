using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IPromocionRepository
    {
        Task<IEnumerable<Promocion>> ObtenerVigentesPorPizzaAsync(int idPizza);
    }
}