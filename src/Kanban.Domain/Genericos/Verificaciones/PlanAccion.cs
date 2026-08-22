using Kanban.Domain.Genericos.Compromisos;

namespace Kanban.Domain.Genericos.Verificaciones;

public class PlanAccion
{
    public int Id { get; set; }
    public int VerificarId { get; set; }
    public string? Descripcion { get; set; }

    public Verificar? Verificar { get; set; }
    public Compromiso? Compromiso { get; set; }
}