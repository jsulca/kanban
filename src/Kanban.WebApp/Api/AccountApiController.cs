using Microsoft.AspNetCore.Authorization;
using Kanban.SharedKernel;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Kanban.Application.Abstractions.UseCases.Administracion;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Kanban.Domain.Genericos.Administracion;
using Kanban.Domain.Genericos.Seguridad;
using Kanban.WebApp.Commons;
using Kanban.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApp.Api;

[ApiController]
[Route("api/account")]
public class AccountApiController(
    IUsuarioLogica usuarioLogica,
    IEstructuraLogica estructuraLogica,
    IIntentoLogica intentoLogica) : ControllerBase
{
    private readonly List<string> _errors = new List<string>();

    #region Acciones

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public IActionResult Login(AccountModel.Get model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {
            Validar(model);
            if (_errors.Count == 0)
            {

                model.Clave = CryptographyHelper.Encrypt(model.Clave);
                Usuario usuario = usuarioLogica.BuscarLogin(model.Nombre, model.Clave);

                if (usuario == null)
                {
                    _errors.Add("No se encontro el usuario con las credenciales ingresadas.");
                    intentoLogica.Guardar(new Intento { Usuario = model.Nombre, Clave = model.Clave, Descripcion = "Usuario y/o clave incorrecta. A través del aplicativo móvil." });
                }
                else if (!usuario.Activo)
                {
                    _errors.Add("El usuario no se encuentra activo.");
                    intentoLogica.Guardar(new Intento { Usuario = model.Nombre, Clave = model.Clave, Descripcion = "El usuario no se encuentra activo. A través del aplicativo móvil." });
                }

                if (_errors.Count > 0)
                {
                    respuesta.response.codigo = "0002";
                    respuesta.response.descripcion = "Error de validación de información.";
                    respuesta.response.comentario = string.Join(",", _errors);
                }
                else
                {
                    bool cambiarClave = false;

                    if (!usuario.CambioClave.HasValue) cambiarClave = true;
                    else if (usuario.DiasVencimiento.HasValue && DateTime.Today > usuario.CambioClave.Value.AddDays(usuario.DiasVencimiento.Value)) cambiarClave = true;

                    string token = TokenGenerator.GenerateTokenJwt(usuario.Id, usuario.Nombre);
                    string ip = GetClientIp();

                    usuarioLogica.Token(usuario.Id, token, ip);

                    if (cambiarClave)
                    {
                        usuario.Id = 0;
                        usuario.Nombre = string.Empty;
                        usuario.RolId = 0;
                        usuario.Rol = new Rol { Nombre = string.Empty };
                        usuario.EmpleadoId = 0;
                        usuario.Empleado = new Empleado { Nombre = string.Empty, ApellidoPaterno = string.Empty };
                        usuario.EstructuraId = 0;
                        usuario.Estructura = new Estructura { Descripcion = string.Empty };
                        usuario.Estructuras = new List<UsuarioEstructura>();
                    }

                    respuesta.response.codigo = "0000";
                    respuesta.response.descripcion = "Ok";
                    respuesta.data = new
                    {
                        usuarioId = usuario.Id,
                        usuario = usuario.Nombre,
                        rolId = usuario.RolId,
                        rol = usuario.Rol.Nombre.ToUpper(),
                        empleadoId = usuario.EmpleadoId,
                        empleado = string.Format("{0} {1}", usuario.Empleado.Nombre, usuario.Empleado.ApellidoPaterno).ToUpper(),
                        estructuraId = usuario.EstructuraId,
                        estructura = usuario.Estructura.Descripcion,
                        ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(usuario.EstructuraId)?.ToUpper() ?? ""),
                        tableros = usuario.Estructuras?.Where(x => x.Estructura.Tablero).Select(x => new { estructuraId = x.EstructuraId, descripcion = x.Estructura.Descripcion }),
                        estructurasId = usuario.Estructuras?.Where(x => x.Acceso).Select(x => x.EstructuraId).ToArray(),
                        token,
                        cambiarClave
                    };
                }
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

    [Route("changepassword")]
    [HttpPost]
    public IActionResult ChangePassword(AccountModel.ChangePassword model)
    {
        ResponseModel respuesta = new ResponseModel();
        try
        {

            Usuario usuario = null;
            int usuarioId = GetUserID();

            Validar(model, usuarioId);
            if (_errors.Count == 0)
            {
                model.NuevaClave = CryptographyHelper.Encrypt(model.NuevaClave);
                usuario = usuarioLogica.BuscarPorId(usuarioId, true);
                if (model.NuevaClave.Equals(usuario.Clave)) _errors.Add("Debe ingresar una clave distinta a la anterior.");
            }
            if (_errors.Count == 0)
            {
                usuario.CambioClave = DateTime.Today;
                usuario.Clave = model.NuevaClave;
                usuarioLogica.CambiarClave(usuario);

                string token = TokenGenerator.GenerateTokenJwt(usuario.Id, usuario.Nombre);
                string ip = GetClientIp();

                usuarioLogica.Token(usuario.Id, token, ip);

                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Ok";
                respuesta.data = new
                {
                    usuarioId = usuario.Id,
                    usuario = usuario.Nombre,
                    rolId = usuario.RolId,
                    rol = usuario.Rol.Nombre.ToUpper(),
                    empleadoId = usuario.EmpleadoId,
                    empleado = string.Format("{0} {1}", usuario.Empleado.Nombre, usuario.Empleado.ApellidoPaterno).ToUpper(),
                    estructuraId = usuario.EstructuraId,
                    estructura = usuario.Estructura.Descripcion,
                    ruta = string.Concat("/ALICORP", estructuraLogica.Ruta(usuario.EstructuraId)?.ToUpper() ?? ""),
                    tableros = usuario.Estructuras?.Where(x => x.Estructura.Tablero).Select(x => new { estructuraId = x.EstructuraId, descripcion = x.Estructura.Descripcion }),
                    estructurasId = usuario.Estructuras?.Where(x => x.Acceso).Select(x => x.EstructuraId).ToArray(),
                    token,
                };
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

    [Route("desactivar")]
    [HttpPost]
    [Authorize]
    public IActionResult Desactivar()
    {
        ResponseModel respuesta = new ResponseModel();

        try
        {
            int usuarioId = GetUserID();

            if(usuarioId == 0)
            {
                respuesta.response.codigo = "0001";
                respuesta.response.descripcion = "Error de validación de datos.";
                respuesta.response.comentario = "El usuario no tiene identificador";
            }
            else
            {
                usuarioLogica.Desactivar(usuarioId);

                respuesta.response.codigo = "0000";
                respuesta.response.descripcion = "Ok";
                respuesta.response.comentario = "El usuario fue desactivado";
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
    private void Validar(AccountModel.Get model)
    {
        if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrWhiteSpace(model.Nombre)) _errors.Add("Ingrese el nombre del usuario");
        if (string.IsNullOrEmpty(model.Clave) || string.IsNullOrWhiteSpace(model.Clave)) _errors.Add("Ingrese una clave");
    }

    [NonAction]
    private int GetUserID()
    {
        var identity = User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            IEnumerable<Claim> claims = identity.Claims;
            string userId = claims.SingleOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value ?? "0";
            return int.Parse(userId);
        }
        else return 0;
    }

    [NonAction]
    private void Validar(AccountModel.ChangePassword model, int usuarioId)
    {
        if (usuarioId <= 0) _errors.Add("El identificador del usuario no es válido.");
        if (!model.NuevaClave.Equals(model.ConfirmacionClave)) _errors.Add("Ambas claves no coinciden.");
        else
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var hasSpecialCharacter = new Regex(@"[!@#$%^&*]+");

            if (!hasUpperChar.IsMatch(model.NuevaClave)) _errors.Add("La clave debe contener un carácter en mayuscula");
            if (!hasNumber.IsMatch(model.NuevaClave)) _errors.Add("La clave debe contener un número");
            if (!hasSpecialCharacter.IsMatch(model.NuevaClave)) _errors.Add("La clave debe contener al menos una carácter especial.");
            if (!hasMinimum8Chars.IsMatch(model.NuevaClave)) _errors.Add("La clave de contener como minimo 8 caracteres");
        }
    }

    /// <summary>
    ///     La IP del cliente ya no hay que rescatarla de las propiedades del
    ///     HttpRequestMessage: la trae la conexion.
    /// </summary>
    [NonAction]
    private string GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
    }
    #endregion

}
