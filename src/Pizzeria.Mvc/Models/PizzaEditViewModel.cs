using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Mvc.Models;

public class PizzaEditViewModel
{
    public int IdPizza { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Range(0.01, 10000000, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }
}