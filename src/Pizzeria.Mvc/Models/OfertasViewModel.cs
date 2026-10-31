using System.ComponentModel.DataAnnotations;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Mvc.Models;

public class PromocionFormViewModel
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    public TipoPromocion Tipo { get; set; } = TipoPromocion.Porcentaje;

    [Range(0.01, 10000000, ErrorMessage = "El valor debe ser mayor a cero.")]
    public decimal Valor { get; set; }

    [Range(1, 100, ErrorMessage = "La cantidad mínima debe ser al menos 1.")]
    public int CantidadMinima { get; set; } = 1;

    public DateTime FechaInicio { get; set; } = DateTime.Now;
    public DateTime FechaFin { get; set; } = DateTime.Now.AddDays(7);
}

public class PromocionItemViewModel
{
    public int IdPromocion { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;   // "20% off", "$1000 menos por pizza"...
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool PuedeDesactivar { get; set; }
}

public class OfertasViewModel
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public List<PromocionItemViewModel> Promociones { get; set; } = new();
    public PromocionFormViewModel Nueva { get; set; } = new();
}