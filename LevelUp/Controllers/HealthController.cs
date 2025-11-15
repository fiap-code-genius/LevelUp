using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace LevelUp.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthService;

        public HealthController(HealthCheckService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet("live")]
        [SwaggerOperation(
            Summary = "Lista a situação da API",
            Description = "Retorna todas as informações necessárias sobre a saúde atual da API."
        )]
        [SwaggerResponse(statusCode: 200, description: "API saudável.")]
        [SwaggerResponse(statusCode: 503, description: "API com problemas.")]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Live(CancellationToken ct)
        {
            var report = await _healthService.CheckHealthAsync(
                r => r.Tags.Contains("live"), ct);

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.ToString(),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message
                })
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(503, result);
        }

        [HttpGet("ready")]
        [SwaggerOperation(
            Summary = "Lista a situação do banco de dados",
            Description = "Retorna todas as informações necessárias sobre a saúde atual do banco de dados."
        )]
        [SwaggerResponse(statusCode: 200, description: "Banco saudável e Online.")]
        [SwaggerResponse(statusCode: 503, description: "Banco com problemas.")]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Ready(CancellationToken ct)
        {
            var report = await _healthService.CheckHealthAsync(
                r => r.Tags.Contains("ready"), ct);

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.ToString(),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message
                })
            };
            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(503, result);
        }
    }
}
