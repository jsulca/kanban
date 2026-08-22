using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Domain.Genericos.Administracion;
using Kanban.WebApp.Commons;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class CargoController(ICargoLogica cargoLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetCargo")
    {
        ViewBag.CallBack = callBack;

        Cargo model = new Cargo { };
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(Cargo model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                //cargoLogica.Guardar(model);
                cargoLogica.GuardarEF(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetCargo")
    {
        try
        {
            ViewBag.CallBack = callBack;

            Cargo model = cargoLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Cargo model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                cargoLogica.Actualizar(model);
                return Content(model.Id.ToString());
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
            List<Cargo> lista = cargoLogica.Listar() ?? new List<Cargo>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Descripcion,
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
    private void Validar(Cargo model)
    {
        ModelState.Clear();
        if (string.IsNullOrWhiteSpace(model.Descripcion)) ModelState.AddModelError("Descripcion", "Ingrese una descripción.");
    }

    #endregion
}
