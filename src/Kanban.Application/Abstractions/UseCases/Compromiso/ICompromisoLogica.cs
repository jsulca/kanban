using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.UseCases.Compromiso;

public interface ICompromisoLogica
{
    PagedResult<Domain.Genericos.Compromiso.Compromiso>
        ListarPorPagina(CompromisoFiltro filtro, int page, int pageSize);

    List<Domain.Genericos.Compromiso.Compromiso> Listar(CompromisoFiltro filtro);

    Domain.Genericos.Compromiso.Compromiso? Buscar(int id, bool conDetalles = false);

    IndicadorCompromisos Indicador(int tableroId, DateTime fechaDesde, DateTime fechaHasta);

    ExportacionCompromisos Exportar(CompromisoFiltro filtro);

    void Guardar(Domain.Genericos.Compromiso.Compromiso entidad, int? usuarioId = null, int? empleadoId = null);

    void Actualizar(Domain.Genericos.Compromiso.Compromiso entidad);

    void CambiarEstado(Domain.Genericos.Compromiso.Compromiso entidad, string? motivo = null, int? usuarioId = null,
        int? empleadoId = null);

    void AsignarAutomatico(Domain.Genericos.Compromiso.Compromiso entidad, int? usuarioId = null,
        int? empleadoId = null);

    void CambiarInstancia(int id, string motivo, int instanciaId, int? usuarioId = null, int? empleadoId = null);

    void Asignar(Domain.Genericos.Compromiso.Compromiso entidad, int? usuarioId = null, int? empleadoId = null);

    /// <summary>Marca como fuera de fecha los compromisos vencidos.</summary>
    void FueraFecha();
}