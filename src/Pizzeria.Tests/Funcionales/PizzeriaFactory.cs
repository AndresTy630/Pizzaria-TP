using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Tests.Funcionales.Fakes;

namespace Pizzeria.Tests.Funcionales;

public class PizzeriaFactory : WebApplicationFactory<Program>
{
    public FakeUsuarioRepository Usuarios { get; } = new();
    public FakePizzaRepository Pizzas { get; } = new();
    public FakePedidoRepository Pedidos { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            Reemplazar<IUsuarioRepository>(services, Usuarios);
            Reemplazar<IPizzaRepository>(services, Pizzas);
            Reemplazar<IPedidoRepository>(services, Pedidos);
            Reemplazar<IEmpleadoRepository>(services, new FakeEmpleadoRepository());
            Reemplazar<ISucursalRepository>(services, new FakeSucursalRepository());
        });
    }

    private static void Reemplazar<T>(IServiceCollection services, T instancia) where T : class
    {
        services.RemoveAll<T>();
        services.AddSingleton(instancia);
    }

    public HttpClient CrearCliente() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,   // para poder verificar a dónde redirige
        HandleCookies = true         // mantiene sesión y cookie antiforgery
    });
}