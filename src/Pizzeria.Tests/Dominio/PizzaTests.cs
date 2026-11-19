using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Tests.Dominio;

public class PizzaTests
{
    [Fact]
    public void Constructor_PizzaNuevaQuedaDisponible()
    {
        var p = new Pizza("Muzza", "Clásica", 100m);

        Assert.True(p.Disponible);
        Assert.Equal(100m, p.Precio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_PrecioNoPositivo_LanzaExcepcion(int precio)
    {
        Assert.Throws<ArgumentException>(() => new Pizza("Muzza", "x", precio));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ActualizarDatos_NombreVacio_LanzaExcepcion(string nombre)
    {
        var p = new Pizza("Muzza", "x", 100m);

        Assert.Throws<ArgumentException>(() => p.ActualizarDatos(nombre, "desc"));
    }

    [Fact]
    public void ActualizarDatos_RecortaEspacios()
    {
        var p = new Pizza("Muzza", "x", 100m);
        p.ActualizarDatos("  Fugazzeta ", "  con cebolla ");

        Assert.Equal("Fugazzeta", p.Nombre);
        Assert.Equal("con cebolla", p.Descripcion);
    }

    [Fact]
    public void MarcarComoAgotada_Y_Disponible_CambianElEstado()
    {
        var p = new Pizza("Muzza", "x", 100m);

        p.MarcarComoAgotada();
        Assert.False(p.Disponible);

        p.MarcarComoDisponible();
        Assert.True(p.Disponible);
    }
}