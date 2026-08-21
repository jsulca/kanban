using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Administracion;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Adicionales;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.UseCases.Administracion;

public class EstructuraLogica(
    IEstructuraRepositorio estructuras,
    IEstructuraEFRepositorio estructurasEf,
    IEstructuraInstanciaRepositorio instancias,
    IEstructuraAreaRepositorio areas,
    IEstructuraEmpleadoRepositorio empleados,
    ISostenibilidadRepositorio sostenibilidades,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : IEstructuraLogica
{
    public List<Estructura> Listar(EstructuraFiltro? filtro = null)
    {
        return estructuras.Listar(filtro);
    }

    public List<Estructura> Arbol(int id)
    {
        return estructuras.Arbol(id);
    }

    public List<EstructuraArea> Listar(int estructuraId)
    {
        return areas.Listar(estructuraId);
    }

    public Estructura? Buscar(int id, bool conDetalles = true)
    {
        var entidad = estructuras.Buscar(id);

        if (conDetalles && entidad is not null)
            entidad.Instancias = instancias.ListarInstancia(id);

        return entidad;
    }

    public bool TieneTablero(int id)
    {
        return estructuras.TieneTablero(id);
    }

    public string? Ruta(int id)
    {
        return estructuras.Ruta(id);
    }

    public List<TableroResumen> Listar(int[] tableros)
    {
        return estructuras.Resumen(tableros);
    }

    public bool Guardar(Estructura entidad)
    {
        return transacciones.Ejecutar(() => estructuras.Guardar(entidad));
    }

    public void GuardarEF(Estructura entidad)
    {
        estructurasEf.Save(entidad);
        unitOfWork.SaveChanges();
    }

    public void Guardar(
        int estructuraId,
        List<EstructuraInstancia> instancias1,
        List<EstructuraArea> areas1,
        List<EstructuraEmpleado> empleados1,
        List<Sostenibilidad> sostenibilidades1)
    {
        transacciones.Ejecutar(() =>
        {
            instancias.Limpiar(estructuraId);
            if (instancias1 != null) instancias.Guardar(instancias1);

            areas.Limpiar(estructuraId);
            if (areas1 != null) areas.Guardar(areas1);

            empleados.Limpiar(estructuraId);
            if (empleados1 != null) empleados.Guardar(empleados1);

            sostenibilidades.Limpiar(estructuraId);
            if (sostenibilidades1 != null) sostenibilidades.Guardar(sostenibilidades1);
        });
    }

    public bool Actualizar(Estructura entidad)
    {
        return transacciones.Ejecutar(() => estructuras.Actualizar(entidad));
    }
}