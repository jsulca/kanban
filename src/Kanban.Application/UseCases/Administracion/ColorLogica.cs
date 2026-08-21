using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class ColorLogica(IColorRepositorio colores, ITransacciones transacciones) : IColorLogica
{
    public List<Color> Listar()
    {
        return colores.Listar();
    }

    public Color? Buscar(int id)
    {
        return colores.Buscar(id);
    }

    public bool Guardar(Color entidad)
    {
        return transacciones.Ejecutar(() => colores.Guardar(entidad));
    }

    public bool Actualizar(Color entidad)
    {
        return transacciones.Ejecutar(() => colores.Actualizar(entidad));
    }
}