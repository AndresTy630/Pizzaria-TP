namespace Pizzeria.Mvc.Models;

public class MenuViewModel
{
    public List<PizzaViewModel> Pizzas { get; set; } = new();
    public string? Buscar { get; set; }
    public string Orden { get; set; } = "nombre";
    public bool SoloOfertas { get; set; }
}