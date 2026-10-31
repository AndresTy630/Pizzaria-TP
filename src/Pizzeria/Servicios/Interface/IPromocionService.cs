using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Servicios.Interface;

public interface IPromocionService
{
    Task<IEnumerable<Promocion>> ObtenerPorPizzaAsync(int idPizza);
    Task<int> CrearAsync(Promocion promocion);
    Task DesactivarAsync(int idPromocion);
    Task<IEnumerable<Promocion>> ObtenerVigentesAsync();
}