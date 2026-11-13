using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Mvc.Models;

public class SucursalCrearViewModel
{
    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(150)]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefono { get; set; }

    // Strings a propósito: evita problemas de coma/punto según la cultura del servidor
    public string? Latitud { get; set; }
    public string? Longitud { get; set; }
}