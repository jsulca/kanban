using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Kanban.WebApp.Commons;

/// <summary>
///     El usuario de la petición. Antes vivía entero en <c>Session["user"]</c>; ahora
///     la cookie de autenticación solo lleva su identificador y el resto se relee de la
///     base de datos cada pocos minutos.
///     <para>
///         Se hace así por dos razones: en una cookie no caben las listas de páginas,
///         controles, menús y estructuras (se enviarían en cada petición, incluidas las
///         de imágenes y hojas de estilo), y con una cookie de un día unos datos
///         congelados en el login quedarían obsoletos durante 24 horas — incluido el
///         hecho de que al usuario lo hayan desactivado.
///     </para>
/// </summary>
public static class UsuarioActual
{
    /// <summary>Cuánto se reutiliza lo leído antes de volver a consultarlo.</summary>
    private static readonly TimeSpan Vigencia = TimeSpan.FromMinutes(10);

    private const string ClaveItems = "Kanban.UserInformation";
    private const string ClaimUserId = "kanban:userid";

    /// <summary>Lo único que se guarda en la cookie al iniciar sesión.</summary>
    public static ClaimsPrincipal CrearPrincipal(Usuario usuario)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.Name, usuario.Nombre ?? ""),
            new(ClaimUserId, usuario.Id.ToString())
        ];

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    /// <summary>
    ///     Devuelve <c>null</c> si no hay sesión iniciada o si el usuario ya no está
    ///     activo, que es lo que comprueba <see cref="SafetyFilter" />.
    /// </summary>
    public static UserInformation? GetUser(this HttpContext context)
    {
        if (context.Items.TryGetValue(ClaveItems, out var guardado)) return (UserInformation?)guardado;

        var user = Construir(context);
        context.Items[ClaveItems] = user;
        return user;
    }

    /// <summary>Descarta lo cacheado de un usuario (al cerrar sesión).</summary>
    public static void OlvidarUsuario(this HttpContext context, int usuarioId)
    {
        context.RequestServices.GetRequiredService<IMemoryCache>().Remove(Clave(usuarioId));
    }

    private static UserInformation? Construir(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true) return null;

        if (!int.TryParse(context.User.FindFirst(ClaimUserId)?.Value, out var usuarioId) || usuarioId == 0)
            return null;

        var cache = context.RequestServices.GetRequiredService<IMemoryCache>();

        return cache.GetOrCreate(Clave(usuarioId), entrada =>
        {
            entrada.AbsoluteExpirationRelativeToNow = Vigencia;
            return Cargar(context.RequestServices, usuarioId);
        });
    }

    private static UserInformation? Cargar(IServiceProvider servicios, int usuarioId)
    {
        var usuario = servicios.GetRequiredService<IUsuarioLogica>().BuscarPorId(usuarioId, true);
        if (usuario is null || !usuario.Activo) return null;

        var rol = servicios.GetRequiredService<IRolLogica>().BuscarPorId(usuario.RolId, true);
        var estructuraLogica = servicios.GetRequiredService<IEstructuraLogica>();

        return new UserInformation
        {
            UserId = usuario.Id,
            UserName = usuario.Nombre,
            RoleId = usuario.RolId,
            Role = usuario.Rol?.Nombre?.ToUpper(),
            EmployeeId = usuario.EmpleadoId,
            Employee = $"{usuario.Empleado?.Nombre} {usuario.Empleado?.ApellidoPaterno}".ToUpper(),
            StructureId = usuario.EstructuraId,
            Structure = usuario.Estructura?.Descripcion,
            AreaId = usuario.Empleado?.AreaId ?? 0,
            Area = usuario.Empleado?.Area?.Descripcion,
            StructureRoute = string.Concat("/ALICORP", estructuraLogica.Ruta(usuario.EstructuraId)?.ToUpper() ?? ""),
            Pages = rol?.Paginas.Select(x => x.Pagina!).ToList() ?? [],
            Controls = rol?.Controles.Select(x => x.Control!).ToList() ?? [],
            Menus = rol?.Menus.Select(x => x.Menu!).ToList() ?? [],
            Structures = usuario.Estructuras
        };
    }

    private static string Clave(int usuarioId)
    {
        return $"kanban:usuario:{usuarioId}";
    }
}
