using Kanban.Domain.Genericos.Compromiso;

namespace Kanban.Application.Abstractions.Repositories.Compromiso;

public interface ICompromisoInstanciaRepositorio
{
    List<CompromisoInstancia> Listar(int compromisoId);

    List<CompromisoInstancia> Exportar(int[] compromisosId);

    bool Guardar(CompromisoInstancia entidad);
}