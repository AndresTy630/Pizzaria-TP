using Moq;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Service;

namespace Pizzeria.Tests.Servicios;

public class PizzaServiceTests
{
    private readonly Mock<IPizzaRepository> _repo = new();
    private readonly PizzaService _service;

    public PizzaServiceTests() => _service = new PizzaService(_repo.Object);

    [Fact]
    public async Task Registrar_NombreVacio_LanzaExcepcion()
    {
        var pizza = new Pizza { IdPizza = 1 };   // nombre vacío, precio 0

        await Assert.ThrowsAsync<ArgumentException>(() => _service.RegistrarPizzaAsync(pizza));
        _repo.Verify(r => r.CrearPizzaAsync(It.IsAny<Pizza>()), Times.Never);
    }

    [Fact]
    public async Task Registrar_Valida_LlamaAlRepositorio()
    {
        _repo.Setup(r => r.CrearPizzaAsync(It.IsAny<Pizza>())).ReturnsAsync(3);

        var id = await _service.RegistrarPizzaAsync(new Pizza("Muzza", "x", 100m));

        Assert.Equal(3, id);
    }

    [Fact]
    public async Task Actualizar_PizzaInexistente_LanzaKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.ActualizarPizzaAsync(99, "x", "y", 10m));
    }

    [Fact]
    public async Task Actualizar_PrecioInvalido_LanzaExcepcion_YNoGuarda()
    {
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Pizza("Muzza", "x", 100m) { IdPizza = 1 });

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ActualizarPizzaAsync(1, "Muzza", "x", 0m));
        _repo.Verify(r => r.ActualizarAsync(It.IsAny<Pizza>()), Times.Never);
    }

    [Fact]
    public async Task CambiarDisponibilidad_AlternaElEstado()
    {
        var pizza = new Pizza("Muzza", "x", 100m) { IdPizza = 1 };
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(pizza);

        await _service.CambiarDisponibilidadAsync(1);
        Assert.False(pizza.Disponible);

        await _service.CambiarDisponibilidadAsync(1);
        Assert.True(pizza.Disponible);
    }

    [Fact]
    public async Task CambiarDisponibilidad_PizzaInexistente_LanzaKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CambiarDisponibilidadAsync(99));
    }
}