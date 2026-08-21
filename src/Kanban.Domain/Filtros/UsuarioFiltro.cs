namespace Kanban.Domain.Filtros;

public class UsuarioFiltro
{
    public string? Nombre { get; set; }
    public string? EmpleadoNombre { get; set; }
    public string? EmpleadoApellidoPaterno { get; set; }
    public string? EmpleadoApellidoMaterno { get; set; }
    public string? RolNombre { get; set; }
    public bool? Activo { get; set; }
}