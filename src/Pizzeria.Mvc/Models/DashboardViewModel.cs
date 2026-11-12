using Pizzeria.Dominio.Enums;

namespace Pizzeria.Mvc.Models;

public class DashboardViewModel
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente";

    public int PizzasDisponibles { get; set; }
    public int PedidosTotales { get; set; }
    public int PedidosEnCurso { get; set; }
    public int PedidosEntregados { get; set; }

    public List<PedidoResumenViewModel> PedidosRecientes { get; set; } = new();
}