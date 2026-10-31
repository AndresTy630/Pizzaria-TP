namespace Pizzeria.Dominio.Entidades;

public class SucursalReparto
{
    public int IdSucursal { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public int Repartidores { get; set; }   // cuántos trabajan en la sucursal
    public int Libres { get; set; }         // cuántos no tienen un reparto en curso
}