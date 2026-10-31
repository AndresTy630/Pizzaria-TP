using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface ISucursalRepository
    {
        Task<IEnumerable<Sucursal>> ObtenerActivasAsync();
        Task<IEnumerable<SucursalReparto>> ObtenerRepartoAsync();
    }
}