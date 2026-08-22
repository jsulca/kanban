using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Common;

/// <summary>
///     Detalle del confirmador de un mes: semanas, comentarios y sostenibilidad.
///     Es la contraparte de lectura de <c>IVerificarLogica.Guardar(semanas, comentarios, meses)</c>.
/// </summary>
public sealed record DetalleConfirmador(
    List<ConfirmadorSemana> Semanas,
    List<ConfirmadorComentario> Comentarios,
    List<SostenibilidadMes> Meses);