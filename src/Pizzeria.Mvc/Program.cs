using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Persistencia.Repositorios;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// =============================
// REPOSITORIOS
// =============================

builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// =============================
// SERVICIOS
// =============================

builder.Services.AddScoped<IPizzaService, PizzaService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// =============================
// HASH DE CONTRASEÑAS
// =============================

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>
>();

// =============================
// AUTENTICACIÓN
// =============================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";

        options.AccessDeniedPath =
            "/Login/AccesoDenegado";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

var app = builder.Build();

// =============================
// MIDDLEWARE
// =============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// =============================
// RUTA PRINCIPAL
// =============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Landing}/{id?}");

app.Run();