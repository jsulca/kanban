using Kanban.Application.Abstractions;
using Kanban.Application.Abstractions.Repositories.Verificacion;
using Kanban.Application.Abstractions.UseCases.Verificacion;
using Kanban.Application.Common;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Verificaciones;

namespace Kanban.Application.UseCases.Verificaciones;

public class VerificarLogica(
    IVerificarRepositorio verificar,
    IVerificarEFRepositorio verificarEf,
    IVerificarRespuestaRepositorio respuestas,
    IVerificarRespuestaEFRepositorio respuestasEf,
    IPlanAccionRepositorio planes,
    IPlanAccionEFRepositorio planesEf,
    IConfirmadorSemanaRepositorio semanas,
    IConfirmadorComentarioRepositorio comentarios,
    ISostenibilidadMesRepositorio meses,
    ITransacciones transacciones,
    IUnitOfWork unitOfWork)
    : IVerificarLogica
{
    public PagedResult<Verificar> ListarPorPagina(VerificarFiltro filter, int pageIndex, int pageSize)
    {
        return verificar.ListarPorPagina(filter, pageIndex, pageSize);
    }

    public List<Verificar> Reporte(int tableroId, DateTime fechaDesde, DateTime fechaHasta)
    {
        return verificar.Reporte(tableroId, fechaDesde, fechaHasta);
    }

    public List<Verificar> TableroResumen(VerificarFiltro filter)
    {
        return verificar.TableroResumen(filter);
    }

    public Verificar? Buscar(int id, bool conDetalles = false)
    {
        var entidad = verificar.Buscar(id);

        if (entidad is not null && conDetalles)
        {
            entidad.Respuestas = respuestas.Listar(id) ?? [];
            entidad.PlanesAccion = planes.Listar(id);
        }

        return entidad;
    }

    public DetalleConfirmador Listar(int estructuraId, int anio, int mes)
    {
        return new DetalleConfirmador(
            semanas.Listar(new ConfirmadorSemanaFiltro { EstructuraId = estructuraId, Anio = anio, Mes = mes }),
            comentarios.Listar(new ConfirmadorComentarioFiltro
                { EstructuraId = estructuraId, Anio = anio, Mes = mes }),
            meses.Listar(new SostenibilidadMesFiltro { EstructuraId = estructuraId, Anio = anio }));
    }

    public void Guardar(Verificar entidad)
    {
        transacciones.Ejecutar(() =>
        {
            if (!verificar.Guardar(entidad) || entidad.Respuestas is null) return;

            foreach (var respuesta in entidad.Respuestas)
            {
                respuesta.VerificarId = entidad.Id;
                respuestas.Guardar(respuesta);
            }

            if (entidad.PlanesAccion is null) return;

            entidad.PlanesAccion.ForEach(x => x.VerificarId = entidad.Id);
            planes.Guardar(entidad.PlanesAccion);
        });
    }

    public void GuardarEF(Verificar entidad)
    {
        entidad.FechaRegistro = DateTime.Now;

        verificarEf.Save(entidad);

        if (entidad.Respuestas != null) respuestasEf.Save(entidad.Respuestas);
        if (entidad.PlanesAccion != null) planesEf.Save(entidad.PlanesAccion);

        unitOfWork.SaveChanges();
    }

    public void Guardar(
        List<ConfirmadorSemana> semanas1,
        List<ConfirmadorComentario> comentarios1,
        List<SostenibilidadMes> meses1)
    {
        transacciones.Ejecutar(() =>
        {
            if (semanas1 != null) semanas.Guardar(semanas1);
            if (comentarios1 != null) comentarios.Guardar(comentarios1);
            if (meses1 != null) meses.Guardar(meses1);
        });
    }
}