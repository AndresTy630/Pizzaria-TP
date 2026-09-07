using Pizzeria.Dominio.Enums;

namespace Pizzeria.Dominio.Entidades;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string UserName {get; set;} = string.Empty;
    public string Pass {get; set;} = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public Roles rol { get; set; } 

    public Usuario() { }
}
