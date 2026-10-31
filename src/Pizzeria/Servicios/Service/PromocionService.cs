using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class PromocionService : IPromocionService
{
    private readonly IPromocionRepository _promocionRepository;
    private readonly IPizzaRepository _pizzaRepository;

    public PromocionService(IPromocionRepository promocionRepository, IPizzaRepository pizzaRepository)
    {
        _promocionRepository = promocionRepository;
        _pizzaRepository = pizzaRepository;
    }

    public async Task<IEnumerable<Promocion>> ObtenerPorPizzaAsync(int idPizza)
        => await _promocionRepository.ObtenerPorPizzaAsync(idPizza);

    public async Task<int> CrearAsync(Promocion promocion)
    {
        var pizza = await _pizzaRepository.ObtenerPorIdAsync(promocion.IdPizza)
            ?? throw new ArgumentException("La pizza no existe.");

        promocion.Validar(pizza.Precio);

        return await _promocionRepository.CrearAsync(promocion);
    }

    public async Task DesactivarAsync(int idPromocion)
        => await _promocionRepository.DesactivarAsync(idPromocion);

    public async Task<IEnumerable<Promocion>> ObtenerVigentesAsync()
    => await _promocionRepository.ObtenerVigentesAsync();
}