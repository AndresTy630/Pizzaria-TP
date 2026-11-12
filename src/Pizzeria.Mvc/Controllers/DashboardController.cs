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
        var rol = User.FindFirstValue(ClaimTypes.Role) ?? "Cliente";

        var pedidos = (await _pedidoService.ObtenerPedidosDeUsuarioAsync(usuarioId))
            .OrderByDescending(p => p.FechaHora)
            .Select(p => new PedidoResumenViewModel
            {
                IdPedido = p.IdPedido,
                Fecha = p.FechaHora,
                Estado = p.Estado,
                TipoEntrega = p.TipoEntrega,
                Total = p.Total
            })
            .ToList();

        var model = new DashboardViewModel
        {
            NombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario",
            Rol = rol,
            PedidosTotales = pedidos.Count,
            PedidosEntregados = pedidos.Count(p => !p.EnCurso),
            TotalGastado = pedidos.Where(p => !p.EnCurso).Sum(p => p.Total),
            EnCurso = pedidos.Where(p => p.EnCurso).ToList(),
            Historial = pedidos.Where(p => !p.EnCurso).Take(5).ToList()
        };

        if (rol == "Admin")
            model.PizzasAgotadas = (await _pizzaService.VerTodasAsync()).Count(p => !p.Disponible);

        return View(model);
    }
}