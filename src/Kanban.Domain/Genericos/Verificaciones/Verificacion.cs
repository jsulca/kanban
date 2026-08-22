using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Domain.Genericos.Verificaciones;

public class Verificacion
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Instruccion { get; set; }
    public bool Rom { get; set; }
    public bool Activo { get; set; }
    public bool Fortaleza { get; set; }
    public bool Oportunidad { get; set; }
    public bool PlanAccion { get; set; }
    public bool InstructivoEstandar { get; set; }
    public bool ResumenCategoria { get; set; }
    public int TipoVerificacionId { get; set; }
    public bool VP { get; set; }
    public bool IGP { get; set; }

    public TipoVerificacion? TipoVerificacion { get; set; }
    public List<Categoria> Categorias { get; set; } = [];
}