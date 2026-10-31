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
    public int? CantidadPaga { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activa { get; set; }

    public decimal CalcularDescuento(decimal precioUnitario, int cantidad)
    {
        if (cantidad < CantidadMinima)
            return 0m;

        var bruto = precioUnitario * cantidad;

        var descuento = Tipo switch
        {
            TipoPromocion.Porcentaje => bruto * Valor / 100m,
            TipoPromocion.MontoFijo => Valor * cantidad,
            TipoPromocion.LlevaPaga when CantidadPaga is > 0 && CantidadPaga < CantidadMinima =>
                (cantidad / CantidadMinima) * (CantidadMinima - CantidadPaga.Value) * precioUnitario,
            _ => 0m
        };

        return Math.Round(Math.Min(descuento, bruto), 2);
    }

    public string Estado(DateTime? ahora = null)
    {
        var fecha = ahora ?? DateTime.Now;

        if (!Activa) return "Desactivada";
        if (fecha < FechaInicio) return "Programada";
        if (fecha > FechaFin) return "Vencida";
        return "Vigente";
    }

    public void Validar(decimal precioPizza)
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new ArgumentException("El título es obligatorio.");

        if (FechaFin <= FechaInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.");

        if (CantidadMinima < 1)
            throw new ArgumentException("La cantidad mínima debe ser al menos 1.");
        
        if (FechaFin <= DateTime.Now)
            throw new ArgumentException("La fecha de fin debe ser futura.");

        if (Tipo == TipoPromocion.Porcentaje && (Valor <= 0 || Valor > 99))
            throw new ArgumentException("El porcentaje debe estar entre 1 y 99.");

        if (Tipo == TipoPromocion.MontoFijo && (Valor <= 0 || Valor > precioPizza * 0.7m))
            throw new ArgumentException("El monto no puede superar el 70% del precio de la pizza.");

        if (Tipo == TipoPromocion.Porcentaje && (Valor <= 0 || Valor > 100))
            throw new ArgumentException("El porcentaje debe estar entre 1 y 100.");

        if (Tipo == TipoPromocion.MontoFijo && (Valor <= 0 || Valor >= precioPizza))
            throw new ArgumentException("El monto debe ser mayor a 0 y menor al precio de la pizza.");
        if (Tipo == TipoPromocion.LlevaPaga && (CantidadMinima < 2 || CantidadPaga is null or < 1 || CantidadPaga >= CantidadMinima))
            throw new ArgumentException("En un NxM, 'paga' debe ser menor que 'lleva' (ej: lleva 2, paga 1).");
    }

    public string Resumen()
    {
        if (Tipo == TipoPromocion.LlevaPaga)
            return $"{CantidadMinima}x{CantidadPaga}";

        var texto = Tipo == TipoPromocion.Porcentaje
            ? $"{Valor:0.##}% off"
            : $"${Valor:N0} menos";

        if (CantidadMinima > 1)
            texto += $" desde {CantidadMinima} pizzas";

        return texto;
    }
}