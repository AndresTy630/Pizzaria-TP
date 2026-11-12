namespace Pizzeria.Mvc.Models;

public class DashboardViewModel
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente";

    public int PedidosTotales { get; set; }
    public int PedidosEntregados { get; set; }
    public decimal TotalGastado { get; set; }

    public List<PedidoResumenViewModel> EnCurso { get; set; } = new();
    public List<PedidoResumenViewModel> Historial { get; set; } = new();

    // Solo para admin
    public int PizzasAgotadas { get; set; }
}