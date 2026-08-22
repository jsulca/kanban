using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Domain.Genericos.Compromisos;

public class Compromiso
{
    public int Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public string? Detalle { get; set; }
    public string? Impacto { get; set; }
    public EstadoCompromiso Estado { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public DateTime? FechaProgramacion { get; set; }
    public DateTime? FechaReprogramacion { get; set; }
    public int EstructuraId { get; set; }
    public int TableroId { get; set; }
    public int? AreaId { get; set; }
    public int? ResponsableId { get; set; }
    public string? Respuesta { get; set; }
    public int? InstanciaId { get; set; }
    public int? FotoId { get; set; }
    public string? Origen { get; set; }
    public string? Accion { get; set; }
    public int? PlanAccionId { get; set; }

    public int UsuarioRegistroId { get; set; }
    public int EmpleadoRegistroId { get; set; }

    public Estructura? Estructura { get; set; }
    public Estructura? Tablero { get; set; }
    public Area? Area { get; set; }
    public Instancia? Instancia { get; set; }
    public Empleado? Responsable { get; set; }
    public Adjunto? Foto { get; set; }

    public Empleado? EmpleadoRegistro { get; set; }
    public Usuario? UsuarioRegistro { get; set; }

    public List<CompromisoEstado> Estados { get; set; } = [];
    public List<CompromisoInstancia> Instancias { get; set; } = [];
}