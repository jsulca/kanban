namespace Kanban.Domain;

public enum EstadoCompromiso
{
    NUEVO = 1,
    PENDIENTE = 2,
    FUERA_DE_FECHA = 3,
    PROGRAMADO = 4,
    REPROGRAMADO = 5,
    POR_VERIFICAR = 6,
    FINALIZADO = 8,
    RECHAZADO = 9
}

public enum InstanciaObligatoria
{
    GERENCIA = 1,
    DIRECCION = 2
}

public enum TipoMenu
{
    NORMAL = 1,
    COLLAPSE = 2,
    HEADER = 3
}

public enum MenuEstatico
{
    GESTION_COMPROMISO = 1,
    GESTION_CONFIRMACION = 2
}

public struct ConfiguracionMaestro
{
    public const string RENOVACION_CLAVE = "RENOVACION_CLAVE";
}

public struct RolMaestro
{
    public const int ADMINISTRADOR = 1;
    public const int REGISTRO_APLICATIVO_MOVIL = 7;
}

public struct EmpleadoMaestro
{
    public const int JUNIOR_SULCA = 1;
}