using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class OrigenController(IOrigenLogica origenLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetOrigen")
    {
        ViewBag.CallBack = callBack;

        Origen model = new Origen { Activo = true };
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Origen model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                origenLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetOrigen")
    {
        try
        {
            ViewBag.CallBack = callBack;

            Origen model = origenLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Origen model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                origenLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar(OrigenFiltro filtro)
    {
        try
        {
            List<Origen> lista = origenLogica.Listar(filtro) ?? new List<Origen>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.Nombre,
                x.Activo
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
    private void Validar(Origen model)
    {
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un nombre");
    }

    #endregion
}
