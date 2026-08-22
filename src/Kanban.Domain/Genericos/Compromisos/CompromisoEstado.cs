using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Domain.Genericos.Compromisos;

public class CompromisoEstado
{
    public int CompromisoId { get; set; }
    public EstadoCompromiso Estado { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? Motivo { get; set; }
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }

    public Compromiso? Compromiso { get; set; }
    public Usuario? Usuario { get; set; }
    public Empleado? Empleado { get; set; }
}