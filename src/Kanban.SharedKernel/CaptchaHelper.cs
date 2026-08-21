using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Kanban.SharedKernel;

/// <summary>
///     Genera el captcha del login. La versión de .NET Framework lo dibujaba con
///     <c>System.Drawing</c>, que ya no está soportado fuera de Windows; se dibuja
///     como SVG para no depender de ninguna librería de imágenes: el
///     <c>&lt;img&gt;</c> de la vista lo muestra igual.
/// </summary>
public class CaptchaHelper
{
    private const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int Ancho = 200;
    private const int Alto = 50;
    private const int Longitud = 6;

    public CaptchaHelper()
    {
        Texto = GenerarTexto();
        Svg = GenerarSvg(Texto);
    }

    public string Texto { get; }

    public string Svg { get; }

    public byte[] ToByteArray()
    {
        return Encoding.UTF8.GetBytes(Svg);
    }

    private static string GenerarTexto()
    {
        var texto = new char[Longitud];
        for (var i = 0; i < Longitud; i++)
            texto[i] = Caracteres[RandomNumberGenerator.GetInt32(Caracteres.Length)];

        return new string(texto);
    }

    private static string GenerarSvg(string texto)
    {
        var svg = new StringBuilder();
        svg.Append(CultureInfo.InvariantCulture,
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{Ancho}\" height=\"{Alto}\" viewBox=\"0 0 {Ancho} {Alto}\">");
        svg.Append(CultureInfo.InvariantCulture, $"<rect width=\"{Ancho}\" height=\"{Alto}\" fill=\"#ffffff\"/>");

        // Rayas de fondo, para dificultar la lectura automática.
        for (var i = 0; i < 4; i++)
            svg.Append(CultureInfo.InvariantCulture,
                $"<line x1=\"0\" y1=\"{RandomNumberGenerator.GetInt32(Alto)}\" x2=\"{Ancho}\" y2=\"{RandomNumberGenerator.GetInt32(Alto)}\" stroke=\"#9e9e9e\" stroke-width=\"1\"/>");

        var paso = Ancho / (texto.Length + 1);
        for (var i = 0; i < texto.Length; i++)
        {
            var x = paso * (i + 1) - 10;
            var y = 34 + RandomNumberGenerator.GetInt32(-6, 7);
            var giro = RandomNumberGenerator.GetInt32(-25, 26);

            svg.Append(CultureInfo.InvariantCulture,
                $"<text x=\"{x}\" y=\"{y}\" font-family=\"Arial,Helvetica,sans-serif\" font-size=\"28\" fill=\"#212121\" transform=\"rotate({giro} {x} {y})\">{texto[i]}</text>");
        }

        svg.Append("</svg>");
        return svg.ToString();
    }
}
