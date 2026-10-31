using Pizzeria.Dominio.Entidades;

namespace Pizzeria.Dominio.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<Empleado?> ObtenerPorUsuarioAsync(int idUsuario);
    }
}