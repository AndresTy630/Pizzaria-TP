using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Mvc.Models;

public class LineaPedidoViewModel
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }

    [Range(0, 50, ErrorMessage = "La cantidad debe estar entre 0 y 50.")]
    public int Cantidad { get; set; }
}

public class CrearPedidoViewModel
{
    [Required(ErrorMessage = "Elegí una sucursal.")]
    public int? IdSucursal { get; set; }

    public TipoEntrega TipoEntrega { get; set; } = TipoEntrega.RetiroEnSucursal;

    [StringLength(150)]
    public string? DireccionEntrega { get; set; }

    public List<LineaPedidoViewModel> Lineas { get; set; } = new();
    public List<SelectListItem> Sucursales { get; set; } = new();
    public List<SucursalReparto> Reparto { get; set; } = new();
}

public class PedidoResumenViewModel
{
    public int IdPedido { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoPedido Estado { get; set; }
    public TipoEntrega TipoEntrega { get; set; }
    public decimal Total { get; set; }
    public bool EnCurso => Estado != EstadoPedido.Entregado;
}

public class PedidoLineaDetalleViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

public class PedidoDetalleViewModel : PedidoResumenViewModel
{
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<PedidoLineaDetalleViewModel> Lineas { get; set; } = new();
}

public static class EstadoTexto
{
    public static string De(EstadoPedido e) => e switch
    {
        EstadoPedido.EsperaDeConfirmacion => "Esperando confirmación",
        EstadoPedido.EnPreparacion => "En preparación",
        EstadoPedido.Listo => "Listo",
        EstadoPedido.EnViaje => "En viaje",
        EstadoPedido.Entregado => "Entregado",
        _ => e.ToString()
    };

    public static string Color(EstadoPedido e) => e switch
    {
        EstadoPedido.Entregado => "success",
        EstadoPedido.EnViaje => "primary",
        EstadoPedido.Listo => "info",
        EstadoPedido.EnPreparacion => "warning",
        _ => "secondary"
    };
}