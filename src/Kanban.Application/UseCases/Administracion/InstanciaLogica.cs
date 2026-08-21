using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class InstanciaLogica(IInstanciaRepositorio instancias, ITransacciones transacciones)
    : IInstanciaLogica
{
    public List<Instancia> Listar()
    {
        return instancias.Listar();
    }

    public Instancia? Buscar(int id)
    {
        return instancias.Buscar(id);
    }

    public bool Guardar(Instancia entidad)
    {
        return transacciones.Ejecutar(() => instancias.Guardar(entidad));
    }

    public bool Actualizar(Instancia entidad)
    {
        return transacciones.Ejecutar(() => instancias.Actualizar(entidad));
    }
}