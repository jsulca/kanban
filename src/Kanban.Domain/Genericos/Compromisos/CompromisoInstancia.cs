using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Domain.Genericos.Compromisos;

public class CompromisoInstancia
{
    public int CompromisoId { get; set; }
    public int InstanciaId { get; set; }
    public string? Motivo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int? UsuarioId { get; set; }
    public int? EmpleadoId { get; set; }

    public Instancia? Instancia { get; set; }
    public Usuario? Usuario { get; set; }
    public Empleado? Empleado { get; set; }
}