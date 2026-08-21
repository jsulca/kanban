namespace Kanban.Domain.Genericos.Administracion;

public class Adjunto
{
    public int Id { get; set; }
    public string? Ruta { get; set; }
    public string? Nombre { get; set; }
    public string? TipoArchivo { get; set; }
    public int Tamano { get; set; }
}