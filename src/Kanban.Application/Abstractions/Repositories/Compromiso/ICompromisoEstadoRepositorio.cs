using Kanban.Domain.Genericos.Compromisos;

namespace Kanban.Application.Abstractions.Repositories.Compromiso;

public interface ICompromisoEstadoRepositorio
{
    List<CompromisoEstado> Listar(int compromisoId);

    List<CompromisoEstado> Exportar(int[] compromisosId);

    bool Guardar(CompromisoEstado entidad);
}