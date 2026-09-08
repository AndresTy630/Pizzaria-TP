namespace Pizzeria.Dominio.Entidades;

public class Empleado
{
    public int IdEmpleado { get; set; }

    public int IdUsuario { get; set; }

    public int IdSucursal { get; set; }

    public int Rol { get; set; }

    public Empleado()
    {
    }
}