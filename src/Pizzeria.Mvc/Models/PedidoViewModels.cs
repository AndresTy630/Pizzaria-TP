using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Mvc.Models;

public class PromoClienteViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string Resumen { get; set; } = string.Empty;
    public int Tipo { get; set; }              // 1 = Porcentaje, 2 = Monto fijo
    public decimal Valor { get; set; }
    public int CantidadMinima { get; set; }
    public int CantidadPaga { get; set; }
}

public class LineaPedidoViewModel
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? OfertaTexto { get; set; }
    public List<PromoClienteViewModel> Promos { get; set; } = new();

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