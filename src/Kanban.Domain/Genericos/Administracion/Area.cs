namespace Kanban.Domain.Genericos.Administracion;

public class Area
{
    public int Id { get; set; }
    public int ColorFondoId { get; set; }
    public int ColorTextoId { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }

    public Color? ColorFondo { get; set; }
    public Color? ColorTexto { get; set; }

    public List<Empleado> Empleados { get; set; } = [];
}