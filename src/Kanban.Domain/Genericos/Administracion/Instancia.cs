namespace Kanban.Domain.Genericos.Administracion;

public class Instancia
{
    public int Id { get; set; }
    public int ColorFondoId { get; set; }
    public int ColorTextoId { get; set; }
    public string? Abreviatura { get; set; }
    public string? Descripcion { get; set; }

    public Color? ColorFondo { get; set; }
    public Color? ColorTexto { get; set; }
}