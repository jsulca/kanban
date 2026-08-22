using Kanban.Domain.Genericos.Administracion;
using Kanban.SharedKernel;

namespace Kanban.Domain.Genericos.Verificaciones;

public class Verificar
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public int VerificacionId { get; set; }
    public int TableroId { get; set; }
    public int UsuarioId { get; set; }
    public int EstructuraId { get; set; }

    public string? Encargado { get; set; }
    public bool? Rom { get; set; }
    public string? NroRom { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? Fortaleza { get; set; }
    public string? Oportunidad { get; set; }
    public int PuntajeMaximo { get; set; }
    public int PuntajeObtenido { get; set; }
    public string? InstructivoEstandar { get; set; }
    public bool VP { get; set; }
    public int? AreaId { get; set; }
    public bool IGP { get; set; }

    public int SemanaMes => FechaRegistro.GetWeekOfMonth();
    public int NumeroMes => FechaRegistro.Month;

    public Empleado? Empleado { get; set; }
    public Verificacion? Verificacion { get; set; }
    public Estructura? Tablero { get; set; }
    public Estructura? Estructura { get; set; }

    public List<VerificarRespuesta> Respuestas { get; set; } = [];
    public List<PlanAccion> PlanesAccion { get; set; } = [];
}