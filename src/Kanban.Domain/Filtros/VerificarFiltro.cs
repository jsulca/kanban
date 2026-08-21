namespace Kanban.Domain.Filtros;

public class VerificarFiltro
{
    public string? EmpleadoNombre { get; set; }
    public string? EmpleadoApellidoPaterno { get; set; }
    public string? EmpleadoApellidoMaterno { get; set; }
    public string? Encargado { get; set; }
    public string? VerificacionNombre { get; set; }
    public int? EmpleadoId { get; set; }
    public string? EstructuraDescripcion { get; set; }
    public string? TableroDescripcion { get; set; }
    public bool? VP { get; set; }

    public int? TableroId { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int[]? EmpleadoIds { get; set; }
}