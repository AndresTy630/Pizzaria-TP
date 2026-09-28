using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Mvc.Models;

public class LoginViewModel
{
    [Required(
        ErrorMessage = "Ingresá tu usuario o email.")]
    [Display(Name = "Usuario o email")]
    public string Login { get; set; } = string.Empty;

    [Required(
        ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;
}