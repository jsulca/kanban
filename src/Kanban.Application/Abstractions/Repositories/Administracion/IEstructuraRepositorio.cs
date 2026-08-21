using Kanban.Domain.Adicionales;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.Repositories.Administracion;

public interface IEstructuraRepositorio
{
    List<Estructura> Listar(EstructuraFiltro? filtro);

    List<Estructura> Arbol(int id);

    Estructura? Buscar(int id);

    bool Guardar(Estructura entidad);

    bool Actualizar(Estructura entidad);

    bool TieneTablero(int id);

    string? Ruta(int id);

    List<TableroResumen> Resumen(int[] tableros);
}