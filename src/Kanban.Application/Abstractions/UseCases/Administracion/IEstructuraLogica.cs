using Kanban.Domain.Adicionales;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;

namespace Kanban.Application.Abstractions.UseCases.Administracion;

public interface IEstructuraLogica
{
    List<Estructura> Listar(EstructuraFiltro? filtro = null);

    List<Estructura> Arbol(int id);

    /// <summary>Áreas asociadas a una estructura. Ojo: es una sobrecarga de Listar.</summary>
    List<EstructuraArea> Listar(int estructuraId);

    Estructura? Buscar(int id, bool conDetalles = true);

    bool TieneTablero(int id);

    string? Ruta(int id);

    /// <summary>Resumen de varios tableros. Ojo: es una sobrecarga de Listar.</summary>
    List<TableroResumen> Listar(int[] tableros);

    bool Guardar(Estructura entidad);

    /// <summary>Guarda vía Entity Framework en lugar de ADO.NET.</summary>
    void GuardarEF(Estructura entidad);

    /// <summary>Reemplaza el detalle completo de una estructura en una sola transacción.</summary>
    void Guardar(
        int estructuraId,
        List<EstructuraInstancia> instancias,
        List<EstructuraArea> areas,
        List<EstructuraEmpleado> empleados,
        List<Sostenibilidad> sostenibilidades);

    bool Actualizar(Estructura entidad);
}