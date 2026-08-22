namespace Kanban.Domain.Genericos.Verificaciones;

public class VerificarRespuesta
{
    public int VerificarId { get; set; }
    public int CategoriaId { get; set; }
    public int PreguntaId { get; set; }
    public string? Descripcion { get; set; }
    public int Valor { get; set; }

    public Verificar? Verificar { get; set; }
    public Categoria? Categoria { get; set; }
    public Pregunta? Pregunta { get; set; }
}