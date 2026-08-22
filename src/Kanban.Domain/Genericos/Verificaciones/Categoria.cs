namespace Kanban.Domain.Genericos.Verificaciones;

public class Categoria
{
    public int Id { get; set; }
    public int VerificacionId { get; set; }
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Eliminado { get; set; }

    public Verificacion? Verificacion { get; set; }

    public List<Pregunta> Preguntas { get; set; } = [];
}