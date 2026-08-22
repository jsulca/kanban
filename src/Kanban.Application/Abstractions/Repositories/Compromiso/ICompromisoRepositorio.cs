using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.Repositories.Compromiso;

public interface ICompromisoRepositorio
{
    PagedResult<Domain.Genericos.Compromisos.Compromiso> ListarPorPagina(CompromisoFiltro? filtro, int page,
        int pageSize);

    List<Domain.Genericos.Compromisos.Compromiso>? Listar(CompromisoFiltro? filtro);

    Domain.Genericos.Compromisos.Compromiso? Buscar(int id);

    List<Domain.Genericos.Compromisos.Compromiso> Exportar(CompromisoFiltro filtro);

    bool Guardar(Domain.Genericos.Compromisos.Compromiso entidad);

    bool Actualizar(Domain.Genericos.Compromisos.Compromiso entidad);

    int Contar(int estructuraId);

    bool CambiarEstado(Domain.Genericos.Compromisos.Compromiso entidad);

    bool CambiarInstancia(int id, int? instanciaId);

    bool Asignar(Domain.Genericos.Compromisos.Compromiso entidad);

    bool ReiniciarFecha(int id);

    void FueraFecha();

    List<Domain.Genericos.Compromisos.Compromiso> IndicadorPorEstado_1_1(int tableroId, DateTime fechaHasta);

    List<Domain.Genericos.Compromisos.Compromiso> IndicadorPorEstado_1_2(int tableroId, DateTime fechaDesde,
        DateTime fechaHasta);

    List<Domain.Genericos.Compromisos.Compromiso> PorTablero(int tableroId);
}