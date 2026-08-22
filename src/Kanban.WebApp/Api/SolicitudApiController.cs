using System.Net;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Api;

[ApiController]
[Route("api/request")]
public class SolicitudApiController(ISolicitudLogica solicitudLogica) : ControllerBase
{
    private readonly List<string> _errors = new List<string>();

    #region Acciones

    [AllowAnonymous]
    [Route("guardar")]
    [HttpPost]
    public IActionResult Guardar(SolicitudModel.Guardar model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Validar(model);
            if (_errors.Count == 0)
            {
                var solicitud = model.Get();

                solicitudLogica.Guardar(solicitud);

                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Nuestro equipo le enviará un correo, donde tendra sus credenciales.";
                respuesta.data = new { id = solicitud.Id };
            }
            else
            {
                respuesta.response.codigo = "0001";
                respuesta.response.descripcion = "Error de validación de datos.";
                respuesta.response.comentario = string.Join(",", _errors);
            }
        }
        catch (Exception ex)
        {
            respuesta.response.codigo = "0003";
            respuesta.response.descripcion = ex.Message;
        }
        return Ok(respuesta);
    }

    #endregion

    #region Metodos y Funciones

    [NonAction]
    private void Validar(SolicitudModel.Guardar model)
    {
        if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrWhiteSpace(model.Nombre)) _errors.Add("Ingrese el nombre");
        if (string.IsNullOrEmpty(model.Apellido) || string.IsNullOrWhiteSpace(model.Apellido)) _errors.Add("Ingrese el apellido");
        if (string.IsNullOrEmpty(model.NroDocumento) || string.IsNullOrWhiteSpace(model.NroDocumento)) _errors.Add("Ingrese el nro. de documento de identidad");
        if (string.IsNullOrEmpty(model.Correo) || string.IsNullOrWhiteSpace(model.Correo)) _errors.Add("Ingrese el correo");
    }

    #endregion

}
