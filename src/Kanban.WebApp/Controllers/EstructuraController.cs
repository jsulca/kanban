using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class EstructuraController(
    IEstructuraLogica estructuraLogica,
    IInstanciaLogica instanciaLogica,
    IAreaLogica areaLogica,
    IEstructuraAreaLogica estructuraAreaLogica,
    IEstructuraInstanciaLogica estructuraInstanciaLogica,
    IEstructuraEmpleadoLogica estructuraEmpleadoLogica,
    IEmpleadoLogica empleadoLogica,
    ISostenibilidadLogica sostenibilidadLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(int? padreId, string callBack = "SetEstructura")
    {
        Estructura model = new Estructura { PadreId = padreId };

        try
        {
            ViewBag.Areas = areaLogica.Listar();

            ViewBag.CallBack = callBack;
            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Estructura model)
    {
        try
        {

            Validar(model);
            if (ModelState.IsValid)
            {
                if (model.Tablero && model.PadreId.HasValue && estructuraLogica.TieneTablero(model.PadreId.Value))
                    ModelState.AddModelError("Tablero", "No puede agregar un tablero dentro de otro.");
            }
            if (ModelState.IsValid)
            {
                //estructuraLogica.Guardar(model);
                estructuraLogica.GuardarEF(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetEstructura")
    {
        try
        {
            Estructura model = estructuraLogica.Buscar(id);
            ViewBag.CallBack = callBack;

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Estructura model)
    {
        try
        {

            Validar(model);
            if (ModelState.IsValid)
            {
                if (model.Tablero && model.PadreId.HasValue && estructuraLogica.TieneTablero(model.PadreId.Value))
                    ModelState.AddModelError("Tablero", "No puede agregar un tablero dentro de otro.");
            }
            if (ModelState.IsValid)
            {
                Estructura item = estructuraLogica.Buscar(model.Id);
                model.Tablero = item.Tablero;

                estructuraLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult ModelarTablero(int id, string callBack = "SetModelarTablero")
    {
        try
        {

            Estructura model = new Estructura { Id = id };
            model.Areas = estructuraAreaLogica.Listar(id) ?? new List<EstructuraArea>();
            model.Instancias = estructuraInstanciaLogica.Listar(id) ?? new List<EstructuraInstancia>();
            model.Empleados = estructuraEmpleadoLogica.Listar(id) ?? new List<EstructuraEmpleado>();
            model.Sostenibilidades = sostenibilidadLogica.Listar(id) ?? new List<Sostenibilidad>();

            ViewBag.Instancias = instanciaLogica.Listar() ?? new List<Instancia>();

            ViewBag.Areas = areaLogica.Listar() ?? new List<Area>();

            ViewBag.Empleados = empleadoLogica.Listar();

            ViewBag.CallBack = callBack;

            return PartialView("_ModelarTablero", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult ModelarTablero(Estructura model)
    {
        try
        {
            Validar(model.Instancias, model.Areas, model.Empleados, model.Sostenibilidades);
            if (ModelState.IsValid)
            {
                if (model.Areas != null) model.Areas.ForEach(x => x.EstructuraId = model.Id);
                if (model.Instancias != null) model.Instancias.ForEach(x => x.EstructuraId = model.Id);
                if (model.Empleados != null) model.Empleados.ForEach(x => x.EstructuraId = model.Id);
                if (model.Sostenibilidades != null) model.Sostenibilidades.ForEach(x => x.EstructuraId = model.Id);

                estructuraLogica.Guardar(model.Id, model.Instancias, model.Areas, model.Empleados, model.Sostenibilidades);

                return Content("Se guardaron los cambios.");
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar()
    {
        try
        {
            List<Estructura> lista = estructuraLogica.Listar();

            string rpta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.PadreId,
                x.Codigo,
                x.Descripcion,
                x.Tablero
            }));
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Arbol(int id)
    {
        try
        {
            List<Estructura> lista = estructuraLogica.Arbol(id);
            string rpta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.PadreId,
                x.Codigo,
                x.Descripcion,
                x.Tablero
            }));
            return Content(rpta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(Estructura model)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "La descripción no puede estar vacio.");
    }

    [NonAction]
    private void Validar(List<EstructuraInstancia> instancias, List<EstructuraArea> areas, List<EstructuraEmpleado> empleados, List<Sostenibilidad> sostenibilidades)
    {
        ModelState.Clear();
        if (instancias != null)
            for (int i = 0; i < instancias.Count; i++)
                if (instancias[i].InstanciaId <= 0) ModelState.AddModelError("Instancia_" + i, string.Format("La instancia {0} no tiene un identificador.", i + 1));

        if (areas != null)
            for (int i = 0; i < areas.Count; i++)
                if (areas[i].AreaId <= 0) ModelState.AddModelError("Area_" + i, string.Format("El área {0} no tiene un identificador.", i + 1));

        if (empleados != null)
            for (int i = 0; i < empleados.Count; i++)
            {
                if (empleados[i].AreaId <= 0) ModelState.AddModelError("Empleado_" + i, string.Format("El empleado {0} no tiene un area como identificador.", i + 1));
                if (empleados[i].EmpleadoId <= 0) ModelState.AddModelError("Empleado_" + i, string.Format("El empleado {0} no tiene un identificador.", i + 1));
            }

        if (sostenibilidades != null)
            for (int i = 0; i < sostenibilidades.Count; i++)
            {
                if (sostenibilidades[i].EmpleadoId <= 0) ModelState.AddModelError("Sostenibilidad_" + i, string.Format("El empleado de sostenibilidad {0} no tiene un identificador.", i + 1));
            }
    }

    #endregion
}
