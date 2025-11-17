using LevelUp.Application.Interfaces;
using LevelUp.Application.UseCases;
using LevelUp.Domain.Interfaces;
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
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("LevelUp.Infra.Data"));
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
                .AddSqlServer(
                        connectionString: configuration.GetConnectionString("DefaultConnection"),
                        name: "sqlserver_query",
                        tags: new[] { "ready" }
                     );

            // Application repositories
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRewardRepository, RewardRepository>();
            services.AddTransient<IRewardRedemptionRepository, RewardRedemptionRepository>();

            // Application UseCases
            services.AddTransient<IAuthUseCase, AuthUseCase>();
            services.AddTransient<ITeamUseCase, TeamUseCase>();
            services.AddTransient<IUserUseCase, UserUseCase>();
            services.AddTransient<IRewardUseCase, RewardUseCase>();
            services.AddTransient<IRewardRedemptionUseCase, RewardRedemptionUseCase>();
        }
    }
}
