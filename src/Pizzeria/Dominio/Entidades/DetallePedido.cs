namespace Pizzeria.Dominio.Entidades;

public class DetallePedido
{
    public int IdPedido { get; set; }
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
