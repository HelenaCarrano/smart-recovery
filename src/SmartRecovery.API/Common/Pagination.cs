namespace SmartRecovery.API.Common;

/// <summary>Normaliza page/pageSize vindos de query string: valores ausentes ou inválidos caem no
/// default de cada endpoint, e pageSize é limitado a <see cref="MaxPageSize"/> para uma única
/// requisição não conseguir pedir a tabela inteira de uma vez.</summary>
public static class Pagination
{
    public const int MaxPageSize = 100;

    public static int NormalizePage(int page) => page <= 0 ? 1 : page;

    public static int NormalizePageSize(int pageSize, int defaultValue) =>
        pageSize <= 0 ? defaultValue : Math.Min(pageSize, MaxPageSize);
}
