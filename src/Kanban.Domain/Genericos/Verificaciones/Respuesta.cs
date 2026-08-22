namespace Kanban.Domain.Genericos.Verificaciones;

public class Respuesta
{
    public int PreguntaId { get; set; }
    public int Valor { get; set; }
    public string? Descripcion { get; set; }

    public Pregunta? Pregunta { get; set; }
}