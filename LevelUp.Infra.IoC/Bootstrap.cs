using LevelUp.Infra.Data.AppData;
using LevelUp.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LevelUp.Infra.IoC
{
    public class Bootstrap
    {
        public static void AddIoC(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseOracle(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
                .AddOracle(
                        connectionString: configuration.GetConnectionString("DefaultConnection"),
                        name: "oracle_query",
                        tags: new[] { "ready" }
                     );

            // Application repositories
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRewardRepository, RewardRepository>();
            services.AddTransient<IRewardRedemptionRepository, RewardRedemptionRepository>();
        }
    }
}
