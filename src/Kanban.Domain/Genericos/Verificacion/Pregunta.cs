namespace Kanban.Domain.Genericos.Verificacion;

public class Pregunta
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public int Orden { get; set; }
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public bool Eliminado { get; set; }

    public Categoria? Categoria { get; set; }
    public List<Respuesta> Respuestas { get; set; } = [];
}