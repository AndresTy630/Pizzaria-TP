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

    public async Task<int> RegistrarPizzaAsync(Pizza pisha)
    {
        if (string.IsNullOrWhiteSpace(pisha.Nombre))
            throw new ArgumentException("El nombre de la pizza es obligatorio.");

        if (pisha.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        return await _pizzaRepository.CrearPizzaAsync(pisha);
    }

    public async Task<IEnumerable<Pizza>> VerTodasAsync()
    {
        return await _pizzaRepository.ObtenerTodasAsync();
    }

    public async Task ActualizarPizzaAsync(int id, string nombre, string descripcion, decimal precio)
    {
        var pizza = await _pizzaRepository.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("La pizza no existe.");

        pizza.ActualizarDatos(nombre, descripcion);
        pizza.ActualizarPrecio(precio);

        await _pizzaRepository.ActualizarAsync(pizza);
    }

    public async Task CambiarDisponibilidadAsync(int id)
    {
        var pizza = await _pizzaRepository.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("La pizza no existe.");

        if (pizza.Disponible) pizza.MarcarComoAgotada();
        else pizza.MarcarComoDisponible();

        await _pizzaRepository.ActualizarAsync(pizza);
    }
}