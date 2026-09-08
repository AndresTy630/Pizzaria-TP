namespace Pizzeria.Dominio.Entidades;

public class Sucursal
{
    public int IdSucursal { get; set; }

    public string Direccion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public bool Activa { get; set; }

    public Sucursal()
    {
    }
}