namespace Kanban.Domain.Genericos.Administracion;

public class Cargo
{
    public int Id { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public List<Empleado> Empleados { get; set; } = [];
}