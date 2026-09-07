using Pizzeria.Dominio.Interfaces;
using Pizzeria.Persistencia.Repos;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Service;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Repositorios
builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// Servicios
builder.Services.AddScoped<IPizzaService, PizzaService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

var app = builder.Build();

// Configuración del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Ruta principal
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Landing}/{id?}");

app.Run();