namespace Kanban.Application.Common;

/// <summary>
///     Resultado de una consulta paginada. Reemplaza el patrón <c>ref int totalRows</c>
///     heredado de .NET Framework 4.8, incompatible con firmas asíncronas.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalRows,
    int Page,
    int PageSize)
{
    public static PagedResult<T> Empty(int page, int pageSize)
    {
        return new PagedResult<T>([], 0, page, pageSize);
    }
}