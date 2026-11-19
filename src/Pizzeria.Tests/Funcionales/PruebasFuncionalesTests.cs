using System.Net;
using System.Text.RegularExpressions;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Tests.Funcionales;

public class PruebasFuncionalesTests
{
    // ---------- helpers ----------

    private static async Task<string> TokenAsync(HttpClient c, string url)
    {
        var html = await c.GetStringAsync(url);
        var m = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(m.Success, $"No se encontró el token antiforgery en {url}");
        return m.Groups[1].Value;
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient c, string url, Dictionary<string, string> campos)
    {
        campos["__RequestVerificationToken"] = await TokenAsync(c, url);
        return await c.PostAsync(url, new FormUrlEncodedContent(campos));
    }

    private static Task<HttpResponseMessage> Registrar(HttpClient c, string usuario = "juan", string email = "juan@mail.com") =>
        PostAsync(c, "/Account/Registro", new()
        {
            ["Usuario"] = usuario, ["Nombre"] = "Juan", ["Apellido"] = "Pérez", ["Email"] = email,
            ["Password"] = "secreto1", ["ConfirmarPassword"] = "secreto1",
            ["Telefono"] = "1122334455", ["Direccion"] = "Calle 1"
        });

    private static Task<HttpResponseMessage> Login(HttpClient c, string login, string pass) =>
        PostAsync(c, "/Account/Login", new() { ["Login"] = login, ["Password"] = pass });

    private static async Task RegistrarYLoguear(HttpClient c)
    {
        Assert.Equal(HttpStatusCode.Redirect, (await Registrar(c)).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Login(c, "juan", "secreto1")).StatusCode);
    }

    private static async Task<string> Html(HttpResponseMessage r) =>
        WebUtility.HtmlDecode(await r.Content.ReadAsStringAsync());   // Razor escapa las tildes

    private static Dictionary<string, string> PedidoForm(int cantidad = 2) => new()
    {
        ["IdSucursal"] = "1",
        ["TipoEntrega"] = ((int)TipoEntrega.RetiroEnSucursal).ToString(),
        ["Lineas[0].IdPizza"] = "1",
        ["Lineas[0].Cantidad"] = cantidad.ToString()
    };

    // ---------- Flujo 1: registrarse e iniciar sesión ----------

    [Fact]
    public async Task Flujo1_Registrarse_Y_IniciarSesion()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();

        var registro = await Registrar(c);
        Assert.Equal(HttpStatusCode.Redirect, registro.StatusCode);
        Assert.Contains("/Account/Login", registro.Headers.Location!.OriginalString);

        var login = await Login(c, "juan", "secreto1");
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        Assert.Contains("/Dashboard", login.Headers.Location!.OriginalString);

        var panel = await c.GetAsync("/Dashboard");
        Assert.Equal(HttpStatusCode.OK, panel.StatusCode);
        Assert.Contains("Juan", await Html(panel));
    }

    // ---------- Flujo 2: crear un pedido y verlo ----------

    [Fact]
    public async Task Flujo2_CrearPedido_RedirigeAlDetalle_YLoMuestra()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();
        await RegistrarYLoguear(c);

        var respuesta = await PostAsync(c, "/Pedidos/Crear", PedidoForm(cantidad: 2));

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        var destino = respuesta.Headers.Location!.OriginalString;
        Assert.Contains("/Pedidos/Detalle", destino);

        var pedido = Assert.Single(factory.Pedidos.Pedidos);
        Assert.Equal(10000m, pedido.Total);   // 2 x $5000

        var detalle = await c.GetAsync(destino);
        Assert.Equal(HttpStatusCode.OK, detalle.StatusCode);
        var html = await Html(detalle);
        Assert.Contains("Muzzarella", html);
        Assert.Contains($"Pedido #{pedido.IdPedido}", html);
    }

    // ---------- Flujo 3: errores de validación se muestran en la vista ----------

    [Fact]
    public async Task Flujo3_PedidoSinPizzas_MuestraElErrorEnLaVista_SinCrearNada()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();
        await RegistrarYLoguear(c);

        var form = PedidoForm();
        form.Remove("Lineas[0].IdPizza");
        form.Remove("Lineas[0].Cantidad");

        var respuesta = await PostAsync(c, "/Pedidos/Crear", form);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("Elegí al menos una pizza", await Html(respuesta));
        Assert.Empty(factory.Pedidos.Pedidos);
    }

    [Fact]
    public async Task Flujo3b_RegistroConEmailDuplicado_MuestraElErrorEnLaVista()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();
        await Registrar(c, "juan", "juan@mail.com");

        var respuesta = await Registrar(c, "otro", "juan@mail.com");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("email ya está registrado", await Html(respuesta));
    }

    [Fact]
    public async Task Flujo3c_LoginConClaveIncorrecta_MuestraElErrorEnLaVista()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();
        await Registrar(c);

        var respuesta = await Login(c, "juan", "claveMala");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("incorrectos", await Html(respuesta));
    }

    // ---------- Resiliencia: la base se cae ----------

    [Fact]
    public async Task Resiliencia_SiLaBaseSeCae_ElUsuarioVeUnMensajeClaro()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();
        await RegistrarYLoguear(c);
        factory.Pedidos.FallarAlCrear = true;

        var respuesta = await PostAsync(c, "/Pedidos/Crear", PedidoForm());

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var html = await Html(respuesta);
        Assert.Contains("No pudimos registrar el pedido", html);
        Assert.DoesNotContain("Base de datos caída", html);
    }

    // ---------- Seguridad ----------

    [Fact]
    public async Task SinSesion_ElPanelRedirigeAlLogin()
    {
        using var factory = new PizzeriaFactory();
        var c = factory.CrearCliente();

        var respuesta = await c.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Contains("/Account/Login", respuesta.Headers.Location!.OriginalString);
    }

    [Fact]
    public async Task UnUsuarioNoPuedeVerElPedidoDeOtro()
    {
        using var factory = new PizzeriaFactory();

        var ana = factory.CrearCliente();
        await Registrar(ana, "ana", "ana@mail.com");
        await Login(ana, "ana", "secreto1");
        var creado = await PostAsync(ana, "/Pedidos/Crear", PedidoForm());
        var urlPedido = creado.Headers.Location!.OriginalString;

        var luis = factory.CrearCliente();
        await Registrar(luis, "luis", "luis@mail.com");
        await Login(luis, "luis", "secreto1");

        Assert.Equal(HttpStatusCode.NotFound, (await luis.GetAsync(urlPedido)).StatusCode);
    }
}