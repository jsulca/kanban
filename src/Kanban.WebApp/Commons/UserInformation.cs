using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Newtonsoft.Json;

namespace Kanban.WebApp.Commons;

/// <summary>
///     Lo que se guarda en <c>Session["user"]</c> tras el login. Las propiedades
///     calculadas van con <c>[JsonIgnore]</c> porque la sesión de ASP.NET Core
///     serializa el objeto (ver <see cref="SessionExtensions" />).
/// </summary>
public class UserInformation
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public int EmployeeId { get; set; }

    public string? Employee { get; set; }

    public int RoleId { get; set; }

    public string? Role { get; set; }

    public int StructureId { get; set; }

    public string? Structure { get; set; }

    public int AreaId { get; set; }

    public string? Area { get; set; }

    public string? StructureRoute { get; set; }

    [JsonIgnore]
    public int[] StructuresId =>
        Structures?.Where(x => x.Acceso).Select(x => x.EstructuraId).ToArray() ?? [];

    public List<Pagina> Pages { get; set; } = [];

    public List<Control> Controls { get; set; } = [];

    public List<Menu> Menus { get; set; } = [];

    public List<UsuarioEstructura> Structures { get; set; } = [];

    [JsonIgnore]
    public List<Estructura> Tables =>
        Structures?.Where(x => x.Estructura is { Tablero: true }).Select(x => x.Estructura!).ToList() ?? [];
}
