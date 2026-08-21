using Kanban.Domain.Genericos.Compromiso;

namespace Kanban.Application.Common;

/// <summary>
///     Los tres conjuntos que alimentan el indicador de un tablero. Sustituye a los
///     tres parámetros <c>ref</c> que usaba la versión de .NET Framework.
/// </summary>
public sealed record IndicadorCompromisos(
    List<Compromiso> PorEstado11,
    List<Compromiso> PorEstado12,
    List<Compromiso> PorTablero);