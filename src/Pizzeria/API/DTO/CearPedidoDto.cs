using Pizzeria.Dominio.Enums;

namespace Pizzeria.API.DTO;

public class CrearPedidoDto
{
    public int IdUsuario { get; set; }
    public int IdSucursal { get; set; }
    public TipoEntrega TipoEntrega { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<DetalleDto> Detalles { get; set; } = new();
}