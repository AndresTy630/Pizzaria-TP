using Pizzeria.Dominio.Enums;

namespace Pizzeria.Dominio.Entidades;

public class Promocion
{
    public int IdPromocion { get; set; }
    public int IdPizza { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TipoPromocion Tipo { get; set; }
    public decimal Valor { get; set; }
    public int CantidadMinima { get; set; } = 1;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; }

    public decimal CalcularDescuento(decimal precioUnitario, int cantidad)
    {
        if (cantidad < CantidadMinima)
            return 0m;

        var bruto = precioUnitario * cantidad;

        var descuento = Tipo == TipoPromocion.Porcentaje
            ? bruto * Valor / 100m
            : Valor * cantidad;

        return Math.Round(Math.Min(descuento, bruto), 2);
    }
}