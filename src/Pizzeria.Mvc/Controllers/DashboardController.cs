using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Dominio.Enums;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;
using System.Security.Claims;

namespace Pizzeria.Mvc.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IPizzaService _pizzaService;
    private readonly IPedidoService _pedidoService;

    public DashboardController(IPizzaService pizzaService, IPedidoService pedidoService)
    {
        _pizzaService = pizzaService;
        _pedidoService = pedidoService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var pizzas = await _pizzaService.VerDisponiblesAsync();
        var pedidos = await _pedidoService.ObtenerPedidosDeUsuarioAsync(usuarioId);

        var model = new DashboardViewModel
        {
            NombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario",
            Rol = User.FindFirstValue(ClaimTypes.Role) ?? "Cliente",

            PizzasDisponibles = pizzas.Count(),
            PedidosTotales = pedidos.Count(),
            PedidosEnCurso = pedidos.Count(p => p.Estado != EstadoPedido.Entregado),
            PedidosEntregados = pedidos.Count(p => p.Estado == EstadoPedido.Entregado),

            PedidosRecientes = pedidos
                .OrderByDescending(p => p.FechaHora)
                .Take(5)
                .Select(p => new PedidoResumenViewModel
                {
                    IdPedido = p.IdPedido,
                    Fecha = p.FechaHora,
                    Estado = p.Estado,
                    TipoEntrega = p.TipoEntrega,
                    Total = p.Total
                })
                .ToList()
        };

        return View(model);
    }
}