namespace Kanban.Domain.Genericos.Verificacion;

public class ConfirmadorSemana
{
    public int EstructuraId { get; set; }
    public int EmpleadoId { get; set; }
    public int TipoVerificacionId { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public int NroSemana { get; set; }
}