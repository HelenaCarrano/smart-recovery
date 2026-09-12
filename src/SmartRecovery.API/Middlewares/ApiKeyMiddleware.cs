namespace SmartRecovery.API.Middlewares;

/// <summary>
/// Exige o header "X-Api-Key" em todo request, exceto Swagger/health (para não travar
/// a documentação nem os probes de infraestrutura). Se "Security:ApiKey" não estiver
/// configurado (ex: testes de integração, ambiente "Testing"), a verificação é pulada —
/// isso é intencional: em portfólio o objetivo é demonstrar o padrão, não travar CI/testes
/// que não têm motivo para conhecer segredo nenhum.
/// </summary>
public class ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
{
    private const string HeaderName = "X-Api-Key";

    private static readonly string[] ExemptPathPrefixes = ["/swagger", "/api/health"];

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        var configuredKey = configuration["Security:ApiKey"];

        if (string.IsNullOrEmpty(configuredKey) || IsExempt(context.Request.Path))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) || providedKey != configuredKey)
        {
            logger.LogWarning("Requisição rejeitada por API key ausente ou inválida em {Path}.", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key ausente ou inválida." });
            return;
        }

        await next(context);
    }

    private static bool IsExempt(PathString path) =>
        ExemptPathPrefixes.Any(prefix => path.StartsWithSegments(prefix));
}
