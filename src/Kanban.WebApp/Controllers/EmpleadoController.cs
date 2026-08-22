using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class EmpleadoController(IEmpleadoLogica empleadoLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetEmpleado")
    {
        try
        {

            var parametros = empleadoLogica.ObtenerParametros();

            ViewBag.Cargos = parametros.Cargos;
            ViewBag.Areas = parametros.Areas;

            ViewBag.CallBack = callBack;
            Empleado model = new Empleado { };

            return PartialView("_Nuevo", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Nuevo(Empleado model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                empleadoLogica.GuardarEF(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetEmpleado")
    {
        try
        {

            var parametros = empleadoLogica.ObtenerParametros();

            ViewBag.Cargos = parametros.Cargos;
            ViewBag.Areas = parametros.Areas;

            ViewBag.CallBack = callBack;

            var model = empleadoLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Empleado model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                empleadoLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [SafetyFilter]
    public ActionResult Masivo()
    {
        try
        {

            var parametros = empleadoLogica.ObtenerParametros();

            ViewBag.Cargos = parametros.Cargos;
            ViewBag.Areas = parametros.Areas;

            return View();
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    public ActionResult Masivo(EmpleadoModel.Masivo model)
    {
        try
        {
            Validar(model.Empleados);
            if (ModelState.IsValid)
            {
                empleadoLogica.Guardar(model.Empleados);
                return Content("Ok");
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar(EmpleadoFiltro filtro)
    {
        try
        {
            List<Empleado> lista = empleadoLogica.Listar(filtro) ?? new List<Empleado>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.CargoId,
                x.AreaId,
                x.Nombre,
                x.ApellidoPaterno,
                x.ApellidoMaterno,
                x.NroDocumento,
                x.Correo,
                x.Telefono,
                Cargo = new { x.Cargo?.Descripcion },
                Area = new { x.Area?.Descripcion }
            }));
            return Content(respuesta, "application/json");
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(Empleado model)
    {
        ModelState.Clear();
        if (model.AreaId <= 0) ModelState.AddModelError("AreaId", "Seleccione un área.");
        if (string.IsNullOrWhiteSpace(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un Nombre.");
        if (string.IsNullOrWhiteSpace(model.ApellidoPaterno)) ModelState.AddModelError("ApellidoPaterno", "Ingrese su apellido paterno.");
        if (string.IsNullOrWhiteSpace(model.ApellidoMaterno)) ModelState.AddModelError("ApellidoMaterno", "Ingrese su apellido materno.");
        if (string.IsNullOrWhiteSpace(model.NroDocumento)) ModelState.AddModelError("NroDocumento", "Ingrese su Nro de DNI.");
    }

    [NonAction]
    private void Validar(List<Empleado>? model)
    {
        ModelState.Clear();
        if (model == null || model.Count <= 0) ModelState.AddModelError("model", "El contenido se encuentra vacio.");
        else
        {
            int i = 1;
            foreach (var item in model)
            {
                if (item.AreaId <= 0) ModelState.AddModelError("AreaId_" + i, string.Format("Empleado Nº {0}: Seleccione un área.", i));
                if (string.IsNullOrWhiteSpace(item.Nombre)) ModelState.AddModelError("Nombre_" + i, string.Format("Empleado Nº {0}: Ingrese un nombre.", i));
                if (string.IsNullOrWhiteSpace(item.ApellidoPaterno)) ModelState.AddModelError("ApellidoPaterno_" + i, string.Format("Empleado Nº {0}: Ingrese su apellido paterno.", i));
                if (string.IsNullOrWhiteSpace(item.ApellidoMaterno)) ModelState.AddModelError("ApellidoMaterno_" + i, string.Format("Empleado Nº {0}: Ingrese su apellido materno.", i));
                if (string.IsNullOrWhiteSpace(item.NroDocumento)) ModelState.AddModelError("NroDocumento_" + i, string.Format("Empleado Nº {0}: Ingrese su nro. de DNI.", i));
                i++;
            }
        }
    }

    #endregion
}
