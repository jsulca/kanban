using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class TipoVerificacionController(ITipoVerificacionLogica tipoVerificacionLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Nuevo(string callBack = "SetTipoVerificacion")
    {
        ViewBag.CallBack = callBack;

        TipoVerificacion model = new TipoVerificacion { Activo = true };
        return PartialView("_Nuevo", model);
    }

    [HttpPost]
    public ActionResult Nuevo(TipoVerificacion model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                tipoVerificacionLogica.Guardar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Editar(int id, string callBack = "SetTipoVerificacion")
    {
        try
        {
            ViewBag.CallBack = callBack;

            TipoVerificacion model = tipoVerificacionLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(TipoVerificacion model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                tipoVerificacionLogica.Actualizar(model);
                return Content(model.Id.ToString());
            }
            else return Validation();
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    public ActionResult Listar(TipoVerificacionFiltro filtro)
    {
        try
        {
            List<TipoVerificacion> lista = tipoVerificacionLogica.Listar(filtro) ?? new List<TipoVerificacion>();
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
    private void Validar(TipoVerificacion model)
    {
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Nombre)) ModelState.AddModelError("Nombre", "Ingrese un nombre");
    }

    #endregion
}
