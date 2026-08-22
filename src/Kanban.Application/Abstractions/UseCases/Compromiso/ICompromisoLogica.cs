using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.UseCases.Compromiso;

public interface ICompromisoLogica
{
    PagedResult<Domain.Genericos.Compromisos.Compromiso>
        ListarPorPagina(CompromisoFiltro filtro, int page, int pageSize);

    List<Domain.Genericos.Compromisos.Compromiso> Listar(CompromisoFiltro filtro);

    Domain.Genericos.Compromisos.Compromiso? Buscar(int id, bool conDetalles = false);

    IndicadorCompromisos Indicador(int tableroId, DateTime fechaDesde, DateTime fechaHasta);

    ExportacionCompromisos Exportar(CompromisoFiltro filtro);

    void Guardar(Domain.Genericos.Compromisos.Compromiso entidad, int? usuarioId = null, int? empleadoId = null);

    void Actualizar(Domain.Genericos.Compromisos.Compromiso entidad);

    void CambiarEstado(Domain.Genericos.Compromisos.Compromiso entidad, string? motivo = null, int? usuarioId = null,
        int? empleadoId = null);

    void AsignarAutomatico(Domain.Genericos.Compromisos.Compromiso entidad, int? usuarioId = null,
        int? empleadoId = null);

    void CambiarInstancia(int id, string motivo, int instanciaId, int? usuarioId = null, int? empleadoId = null);

    void Asignar(Domain.Genericos.Compromisos.Compromiso entidad, int? usuarioId = null, int? empleadoId = null);

    /// <summary>Marca como fuera de fecha los compromisos vencidos.</summary>
    void FueraFecha();
}