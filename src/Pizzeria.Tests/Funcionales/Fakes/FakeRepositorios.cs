using System.Reflection;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;

namespace Pizzeria.Tests.Funcionales.Fakes;

public class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios = new();
    private int _id;

    public Task<int> CrearUsuarioAsync(Usuario user)
    {
        user.IdUsuario = ++_id;
        _usuarios.Add(user);
        return Task.FromResult(user.IdUsuario);
    }

    public Task<Usuario?> ObtenerPorIdAsync(int id) =>
        Task.FromResult(_usuarios.FirstOrDefault(u => u.IdUsuario == id));

    public Task<Usuario?> ObtenerPorEmailAsync(string email) =>
        Task.FromResult(_usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

    public Task<Usuario?> ObtenerPorUsuarioAsync(string usuario) =>
        Task.FromResult(_usuarios.FirstOrDefault(u => u.NombreUsuario.Equals(usuario, StringComparison.OrdinalIgnoreCase)));
}

public class FakePizzaRepository : IPizzaRepository
{
    private readonly List<Pizza> _pizzas = new()
    {
        new Pizza("Muzzarella", "Clásica con muzzarella", 5000m) { IdPizza = 1 },
        new Pizza("Fugazzeta", "Con cebolla", 6000m) { IdPizza = 2 }
    };

    public Task<IEnumerable<Pizza>> ObtenerDisponiblesAsync() =>
        Task.FromResult<IEnumerable<Pizza>>(_pizzas.Where(p => p.Disponible).ToList());

    public Task<IEnumerable<Pizza>> ObtenerTodasAsync() =>
        Task.FromResult<IEnumerable<Pizza>>(_pizzas.ToList());

    public Task<Pizza?> ObtenerPorIdAsync(int id) =>
        Task.FromResult(_pizzas.FirstOrDefault(p => p.IdPizza == id));

    public Task<int> CrearPizzaAsync(Pizza pisha)
    {
        pisha.IdPizza = _pizzas.Count + 1;
        _pizzas.Add(pisha);
        return Task.FromResult(pisha.IdPizza);
    }

    public Task ActualizarAsync(Pizza pizza) => Task.CompletedTask;
}

public class FakePedidoRepository : IPedidoRepository
{
    private readonly List<Pedido> _pedidos = new();
    private int _id;

    /// <summary>Simula una caída de la base de datos al crear pedidos.</summary>
    public bool FallarAlCrear { get; set; }

    public IReadOnlyList<Pedido> Pedidos => _pedidos;

    public Task<int> CrearPedidoAsync(Pedido pedido)
    {
        if (FallarAlCrear) throw new InvalidOperationException("Base de datos caída (simulada)");

        pedido.IdPedido = ++_id;
        _pedidos.Add(pedido);
        return Task.FromResult(pedido.IdPedido);
    }

    public Task<Pedido?> ObtenerPorIdAsync(int id) =>
        Task.FromResult(_pedidos.FirstOrDefault(p => p.IdPedido == id));

    public Task ActualizarEstadoAsync(int idPedido, EstadoPedido nuevoEstado)
    {
        var p = _pedidos.FirstOrDefault(x => x.IdPedido == idPedido);
        // Estado tiene setter privado: lo seteamos por reflexión
        typeof(Pedido).GetProperty(nameof(Pedido.Estado), BindingFlags.Instance | BindingFlags.Public)!
                      .SetValue(p, nuevoEstado);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Pedido>> ObtenerPorUsuarioAsync(int idUsuario) =>
        Task.FromResult<IEnumerable<Pedido>>(_pedidos.Where(p => p.IdUsuario == idUsuario).ToList());
}

public class FakeEmpleadoRepository : IEmpleadoRepository
{
    public Task<Empleado?> ObtenerPorUsuarioAsync(int idUsuario) => Task.FromResult<Empleado?>(null);
}

public class FakeSucursalRepository : ISucursalRepository
{
    public Task<IEnumerable<Sucursal>> ObtenerActivasAsync() =>
        Task.FromResult<IEnumerable<Sucursal>>(new[]
        {
            new Sucursal { IdSucursal = 1, Direccion = "Av. Siempre Viva 123", Activa = true }
        });

    public Task<IEnumerable<SucursalReparto>> ObtenerRepartoAsync() =>
        Task.FromResult<IEnumerable<SucursalReparto>>(new[]
        {
            new SucursalReparto { IdSucursal = 1, Direccion = "Av. Siempre Viva 123", Repartidores = 2, Libres = 2 }
        });

    public Task<int> CrearAsync(Sucursal sucursal) => Task.FromResult(1);
    public Task<IEnumerable<int>> ObtenerRepartidoresAsync(int idSucursal) =>
    Task.FromResult<IEnumerable<int>>(new[] { 1, 2 });
}