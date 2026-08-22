using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Domain.Genericos.Compromisos;

public class Alerta
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int CompromisoId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Visto { get; set; }

    public Empleado? Empleado { get; set; }
    public Compromiso? Compromiso { get; set; }
}