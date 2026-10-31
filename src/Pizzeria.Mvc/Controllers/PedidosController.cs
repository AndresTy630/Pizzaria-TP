using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Mvc.Models;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Mvc.Controllers;

[Authorize]
public class PedidosController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IPizzaService _pizzaService;
    private readonly IPromocionService _promocionService;
    private readonly IUsuarioService _usuarioService;
    private readonly ISucursalRepository _sucursalRepository;

    public PedidosController(
        IPedidoService pedidoService,
        IPizzaService pizzaService,
        IPromocionService promocionService,
        IUsuarioService usuarioService,
        ISucursalRepository sucursalRepository)
    {
        _pedidoService = pedidoService;
        _pizzaService = pizzaService;
        _promocionService = promocionService;
        _usuarioService = usuarioService;
        _sucursalRepository = sucursalRepository;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new CrearPedidoViewModel();

        var usuario = await _usuarioService.ObtenerUsuarioAsync(UsuarioId);
        model.DireccionEntrega = usuario?.Direccion;

        await CargarOpcionesAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearPedidoViewModel model)
    {
        var lineas = model.Lineas
            .Where(l => l.Cantidad > 0)
            .Select(l => new LineaPedido(l.IdPizza, l.Cantidad))
            .ToList();

        if (!lineas.Any())
            ModelState.AddModelError(string.Empty, "Elegí al menos una pizza.");

        if (model.TipoEntrega == TipoEntrega.Delivery && string.IsNullOrWhiteSpace(model.DireccionEntrega))
            ModelState.AddModelError(nameof(model.DireccionEntrega), "Ingresá la dirección de entrega.");

        if (ModelState.IsValid)
        {
            try
            {
                var id = await _pedidoService.CrearPedidoAsync( UsuarioId, model.IdSucursal!.Value, model.TipoEntrega, model.DireccionEntrega, lineas);

                TempData["Exito"] = "¡Pedido recibido! Ya empezamos a prepararlo.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "No pudimos registrar el pedido. Probá de nuevo en unos minutos.");
            }
        }

        await CargarOpcionesAsync(model);
        return View(model);
    }

    // ---------- MIS PEDIDOS ----------

    public async Task<IActionResult> Index()
    {
        var pedidos = await _pedidoService.ObtenerPedidosDeUsuarioAsync(UsuarioId);

        var model = pedidos.Select(p => new PedidoResumenViewModel
        {
            IdPedido = p.IdPedido,
            Fecha = p.FechaHora,
            Estado = p.Estado,
            TipoEntrega = p.TipoEntrega,
            Total = p.Total
        }).ToList();

        return View(model);
    }

    // ---------- SEGUIMIENTO ----------

    public async Task<IActionResult> Detalle(int id)
    {
        var pedido = await _pedidoService.ObtenerPedidoAsync(id);

        if (pedido is null || pedido.IdUsuario != UsuarioId)
            return NotFound();

        var nombres = (await _pizzaService.VerTodasAsync()).ToDictionary(p => p.IdPizza, p => p.Nombre);

        return View(new PedidoDetalleViewModel
        {
            IdPedido = pedido.IdPedido,
            Fecha = pedido.FechaHora,
            Estado = pedido.Estado,
            TipoEntrega = pedido.TipoEntrega,
            Total = pedido.Total,
            DireccionEntrega = pedido.DireccionEntrega,
            Lineas = pedido.Detalles.Select(d => new PedidoLineaDetalleViewModel
            {
                Nombre = nombres.TryGetValue(d.IdPizza, out var n) ? n : $"Pizza #{d.IdPizza}",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Descuento = d.Descuento
            }).ToList()
        });
    }

    // Lo consulta el JavaScript cada pocos segundos
    [HttpGet]
    public async Task<IActionResult> Estado(int id)
    {
        var pedido = await _pedidoService.ObtenerPedidoAsync(id);

        if (pedido is null || pedido.IdUsuario != UsuarioId)
            return NotFound();

        return Json(new
        {
            estado = (int)pedido.Estado,
            texto = EstadoTexto.De(pedido.Estado),
            color = EstadoTexto.Color(pedido.Estado)
        });
    }

    // Recarga lo que no viaja en el formulario, conservando las cantidades elegidas.
    private async Task CargarOpcionesAsync(CrearPedidoViewModel model)
    {
        var cantidades = model.Lineas.ToDictionary(l => l.IdPizza, l => l.Cantidad);

        var pizzas = await _pizzaService.VerDisponiblesAsync();
        var promos = (await _promocionService.ObtenerVigentesAsync()).ToLookup(p => p.IdPizza);

        model.Lineas = pizzas.Select(p => new LineaPedidoViewModel
        {
            IdPizza = p.IdPizza,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            OfertaTexto = promos[p.IdPizza].Any()
                ? string.Join(" · ", promos[p.IdPizza].Select(x => x.Resumen()))
                : null,
            Promos = promos[p.IdPizza].Select(x => new PromoClienteViewModel
            {
                Titulo = x.Titulo,
                Resumen = x.Resumen(),
                Tipo = (int)x.Tipo,
                Valor = x.Valor,
                CantidadMinima = x.CantidadMinima,
                CantidadPaga = x.CantidadPaga ?? 0
            }).ToList(),
            Cantidad = cantidades.TryGetValue(p.IdPizza, out var c) ? c : 0
        }).ToList();

        var reparto = (await _sucursalRepository.ObtenerRepartoAsync()).ToList();

        model.Reparto = reparto;
        model.Sucursales = reparto
            .Select(s => new SelectListItem(s.Direccion, s.IdSucursal.ToString()))
            .ToList();
    }
}