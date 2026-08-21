using Newtonsoft.Json;

namespace Kanban.WebApp.Controllers;

[SafetyFilter(NoValidarAccion = true)]
public class ConfiguracionController(IConfiguracionLogica configuracionLogica) : AlicorpController
{

    #region Acciones

    [SafetyFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Editar(string id, string callBack = "SetOrigen")
    {
        try
        {
            ViewBag.CallBack = callBack;

            Configuracion model = configuracionLogica.Buscar(id);

            return PartialView("_Editar", model);
        }
        catch (Exception ex)
        {
            return Validation(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult Editar(Configuracion model)
    {
        try
        {
            Validar(model);
            if (ModelState.IsValid)
            {
                configuracionLogica.Actualizar(model);
                return Content(model.Llave);
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
            List<Configuracion> lista = configuracionLogica.Listar() ?? new List<Configuracion>();
            string respuesta = JsonConvert.SerializeObject(lista.Select(x => new
            {
                x.Llave,
                x.Descripcion,
                x.Dias
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
    private void Validar(Configuracion model)
    {
        ModelState.Clear();
        if (string.IsNullOrEmpty(model.Llave)) ModelState.AddModelError("Llave", "La configuracion no tiene una llave");
        if (model.Dias <= 0) ModelState.AddModelError("Dias", "La cantidad de dias no puede ser menor igual a cero");
    }
    #endregion
}
