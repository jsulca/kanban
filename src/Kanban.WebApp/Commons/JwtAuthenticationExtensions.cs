using System.Security.Claims;
using System.Text;
using Kanban.Application.Abstractions.UseCases.Seguridad;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Kanban.WebApp.Commons;

/// <summary>
///     La web se autentica con cookie y <c>/api</c> con JWT, como en .NET Framework
///     (allí eran dos mundos separados: MVC con su sesión y Web API 2 con su
///     <c>TokenValidationHandler</c>). Aquí conviven mediante un esquema que elige uno
///     u otro según la ruta, así que ni los controladores web ni los de la API tienen
///     que declarar nada.
/// </summary>
public static class JwtAuthenticationExtensions
{
    private const string EsquemaSegunRuta = "Kanban";

    /// <summary>Cuánto dura la sesión web sin actividad.</summary>
    private static readonly TimeSpan DuracionCookie = TimeSpan.FromDays(1);

    public static IServiceCollection AddKanbanAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(EsquemaSegunRuta)
            .AddPolicyScheme(EsquemaSegunRuta, "Cookie para la web, JWT para /api", options =>
            {
                options.ForwardDefaultSelector = context =>
                    context.Request.Path.StartsWithSegments("/api")
                        ? JwtBearerDefaults.AuthenticationScheme
                        : CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "Kanban.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;

                options.ExpireTimeSpan = DuracionCookie;
                options.SlidingExpiration = true;

                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.ReturnUrlParameter = "ReturnUrl";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = AppSettings.JWT_AUDIENCE_TOKEN,
                    ValidIssuer = AppSettings.JWT_ISSUER_TOKEN,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JWT_SECRET_KEY))
                };

                // Ojo: el token lleva el claim como "primarysid" y es MapInboundClaims
                // (activo por defecto) quien lo convierte de vuelta a
                // ClaimTypes.PrimarySid. Si se desactiva, GetUserID() de los
                // controladores de /api empezaría a devolver 0 sin avisar.
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var usuarioLogica = context.HttpContext.RequestServices.GetRequiredService<IUsuarioLogica>();

                        var userId = context.Principal?.FindFirst(ClaimTypes.PrimarySid)?.Value ?? "0";
                        var token = LeerToken(context.Request);
                        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                        if (!usuarioLogica.ValidarToken(int.Parse(userId), token, ip))
                            context.Fail("El token no es el vigente del usuario.");

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    private static string LeerToken(HttpRequest request)
    {
        string valor = request.Headers.Authorization.ToString();
        return valor.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? valor[7..] : valor;
    }
}
