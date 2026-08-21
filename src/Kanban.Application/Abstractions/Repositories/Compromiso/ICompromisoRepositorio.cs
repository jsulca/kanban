using Kanban.Application.Common;
using Kanban.Domain.Filtros;

namespace Kanban.Application.Abstractions.Repositories.Compromiso;

public interface ICompromisoRepositorio
{
    PagedResult<Domain.Genericos.Compromiso.Compromiso> ListarPorPagina(CompromisoFiltro? filtro, int page,
        int pageSize);

    List<Domain.Genericos.Compromiso.Compromiso>? Listar(CompromisoFiltro? filtro);

    Domain.Genericos.Compromiso.Compromiso? Buscar(int id);

    List<Domain.Genericos.Compromiso.Compromiso> Exportar(CompromisoFiltro filtro);

    bool Guardar(Domain.Genericos.Compromiso.Compromiso entidad);

    bool Actualizar(Domain.Genericos.Compromiso.Compromiso entidad);

    int Contar(int estructuraId);

    bool CambiarEstado(Domain.Genericos.Compromiso.Compromiso entidad);

    bool CambiarInstancia(int id, int? instanciaId);

    bool Asignar(Domain.Genericos.Compromiso.Compromiso entidad);

    bool ReiniciarFecha(int id);

    void FueraFecha();

    List<Domain.Genericos.Compromiso.Compromiso> IndicadorPorEstado_1_1(int tableroId, DateTime fechaHasta);

    List<Domain.Genericos.Compromiso.Compromiso> IndicadorPorEstado_1_2(int tableroId, DateTime fechaDesde,
        DateTime fechaHasta);

    List<Domain.Genericos.Compromiso.Compromiso> PorTablero(int tableroId);
}