using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Servicios.Interface;

public interface IPromocionService
{
    Task<IEnumerable<Promocion>> ObtenerPorPizzaAsync(int idPizza);
    Task<IEnumerable<Promocion>> ObtenerVigentesAsync();
    Task<int> CrearAsync(Promocion promocion);
    Task DesactivarAsync(int idPromocion);
    Task<(Promocion? Promo, decimal Descuento)> MejorParaAsync(int idPizza, decimal precio, int cantidad);
}