using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Filtros;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class IndicadorController(IIndicadorLogica indicadorLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetIndicador")
    {
        ViewBag.CallBack = callBack;

        Indicador model = new Indicador { Activo = true };
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Indicador model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                indicadorLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetIndicador")
    {
        try
        {
            ViewBag.CallBack = callBack;

            Indicador model = indicadorLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Indicador model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                indicadorLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar(IndicadorFiltro filtro)
    {
        try
        {
            List<Indicador> lista = indicadorLogica.Listar(filtro) ?? new List<Indicador>();
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
    private void Validar(Indicador model)
    {
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un nombre");
    }

    #endregion
}
