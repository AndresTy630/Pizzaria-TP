namespace Pizzeria.Mvc.Models;

public class PizzaGestionViewModel
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Disponible { get; set; }
}