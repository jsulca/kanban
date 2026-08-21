namespace Kanban.Domain.Genericos.Seguridad;

public class Pagina
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Area { get; set; }
    public string? Controlador { get; set; }
    public string? Accion { get; set; }

    public List<Control> Controles { get; set; } = [];
}