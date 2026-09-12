using Microsoft.AspNetCore.Mvc;

namespace SmartRecovery.API.Controllers;

/// <summary>
/// Endpoint de verificação de saúde da API.
/// Útil para monitoramento e para verificar se o serviço está de pé no Docker.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Retorna o status de saúde da API.
    /// </summary>
    /// <returns>Status 200 com informações básicas do serviço.</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Smart Recovery API",
            version = "1.0.0",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
