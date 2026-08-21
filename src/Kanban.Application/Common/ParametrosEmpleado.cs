using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Common;

/// <summary>
///     Catálogos necesarios para el formulario de empleado.
/// </summary>
public sealed record ParametrosEmpleado(
    List<Cargo> Cargos,
    List<Area> Areas);