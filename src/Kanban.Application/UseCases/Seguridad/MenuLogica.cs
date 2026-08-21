using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Seguridad;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain.Genericos.Seguridad;

namespace Kanban.Application.UseCases.Seguridad;

public class MenuLogica(IMenuRepositorio menus, ITransacciones transacciones) : IMenuLogica
{
    public List<Menu> Listar()
    {
        return menus.Listar() ?? [];
    }

    public Menu? BuscarPorId(int id)
    {
        return menus.Buscar(id);
    }

    public void Guardar(Menu entidad)
    {
        transacciones.Ejecutar(() => menus.Guardar(entidad));
    }

    public void Actualizar(Menu entidad)
    {
        transacciones.Ejecutar(() => menus.Actualizar(entidad));
    }
}