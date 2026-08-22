using Kanban.Domain.Genericos.Compromisos;

namespace Kanban.Application.Common;

/// <summary>
///     Compromisos a exportar junto con sus estados e instancias. Los dos últimos
///     llegan vacíos cuando la consulta no devuelve compromisos.
/// </summary>
public sealed record ExportacionCompromisos(
    List<Compromiso> Compromisos,
    List<CompromisoEstado> Estados,
    List<CompromisoInstancia> Instancias);