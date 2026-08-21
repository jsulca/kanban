namespace Kanban.Domain.Genericos.Administracion;

public class Estructura
{
    public int Id { get; set; }
    public int? PadreId { get; set; }
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public bool Tablero { get; set; }

    public List<EstructuraInstancia> Instancias { get; set; } = [];
    public List<EstructuraArea> Areas { get; set; } = [];
    public List<EstructuraEmpleado> Empleados { get; set; } = [];
    public List<Sostenibilidad> Sostenibilidades { get; set; } = [];

    public List<Compromiso.Compromiso> Compromisos { get; set; } = [];
}