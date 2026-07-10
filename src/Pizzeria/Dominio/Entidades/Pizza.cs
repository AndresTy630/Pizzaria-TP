namespace Pizzeria.Dominio.Entidades;

public class Pizza
{
    public int IdPizza { get; set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Precio { get; private set; }
    public bool Disponible { get; private set; }

    public Pizza() { }
    public Pizza(string nombre, string descripcion, decimal precio)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        ActualizarPrecio(precio);
        Disponible = true;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        Precio = nuevoPrecio;
    }

    public void MarcarComoAgotada() => Disponible = false;

    public void MarcarComoDisponible() => Disponible = true;
}
