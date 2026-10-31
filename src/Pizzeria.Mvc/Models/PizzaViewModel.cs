namespace Pizzeria.Mvc.Models;

public class PizzaViewModel
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }                 // precio original

    public decimal PrecioFinal { get; set; }            // con descuento si aplica
    public string? OfertaTexto { get; set; }            // "20% off", "15% off desde 3 pizzas"
    public DateTime? OfertaHasta { get; set; }

    public bool TieneOferta => OfertaTexto is not null;
    public bool TieneDescuento => PrecioFinal > 0 && PrecioFinal < Precio;
}