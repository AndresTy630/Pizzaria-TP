using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class PizzaService : IPizzaService
{
    private readonly IPizzaRepository _pizzaRepository;

    public PizzaService(IPizzaRepository pizzaRepository)
    {
        _pizzaRepository = pizzaRepository;
    }

    public async Task<IEnumerable<Pizza>> VerDisponiblesAsync()
    {
        return await _pizzaRepository.ObtenerDisponiblesAsync();
    }

    public async Task<Pizza?> ObtenerPizzaAsync(int id)
    {
        return await _pizzaRepository.ObtenerPorIdAsync(id);
    }
}
