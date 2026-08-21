using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Domain.Genericos.Seguridad;

public class Usuario
{
    public int Id { get; set; }
    public int RolId { get; set; }
    public int EmpleadoId { get; set; }
    public string? Nombre { get; set; }
    public string? Clave { get; set; }
    public bool Activo { get; set; }
    public int EstructuraId { get; set; }
    public string? Token { get; set; }

    public DateTime? CambioClave { get; set; }
    public int? DiasVencimiento { get; set; }

    public Rol? Rol { get; set; }
    public Empleado? Empleado { get; set; }
    public Estructura? Estructura { get; set; }
    public List<UsuarioEstructura> Estructuras { get; set; } = [];
}