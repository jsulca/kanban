using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.Abstractions.UseCases.Verificacion;

public interface IVerificarLogica
{
    PagedResult<Verificar> ListarPorPagina(VerificarFiltro filter, int pageIndex, int pageSize);

    List<Verificar> Reporte(int tableroId, DateTime fechaDesde, DateTime fechaHasta);

    List<Verificar> TableroResumen(VerificarFiltro filter);

    Verificar? Buscar(int id, bool conDetalles = false);

    /// <summary>Detalle del confirmador de un mes. Ojo: es una sobrecarga de Listar.</summary>
    DetalleConfirmador Listar(int estructuraId, int anio, int mes);

    void Guardar(Verificar entidad);

    /// <summary>Guarda vía Entity Framework en lugar de ADO.NET.</summary>
    void GuardarEF(Verificar entidad);

    /// <summary>Reemplaza el detalle del confirmador en una sola transacción.</summary>
    void Guardar(
        List<ConfirmadorSemana> semanas,
        List<ConfirmadorComentario> comentarios,
        List<SostenibilidadMes> meses);
}