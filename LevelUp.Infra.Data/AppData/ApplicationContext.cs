using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.AppData
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}
