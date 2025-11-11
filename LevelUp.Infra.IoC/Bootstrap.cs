using LevelUp.Infra.Data.AppData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LevelUp.Infra.IoC
{
    public class Bootstrap
    {
        public static void AddIoC(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseOracle(configuration.GetConnectionString("Oracle"));
            });

        }
    }
}
