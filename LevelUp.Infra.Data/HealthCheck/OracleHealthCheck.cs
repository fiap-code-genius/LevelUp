using LevelUp.Infra.Data.AppData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LevelUp.Infra.Data.HealthCheck
{
    public class OracleHealthCheck : IHealthCheck
    {
        private readonly ApplicationContext _context;

        public OracleHealthCheck(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Users.AsNoTracking().Take(1).AnyAsync(cancellationToken);

                return HealthCheckResult.Healthy("Banco de dados Oracle está online.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Banco de dados Oracle está offline.", ex);
            }
        }
    }
}
