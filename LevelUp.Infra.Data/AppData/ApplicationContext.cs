using LevelUp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.AppData
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(e =>
            {
                e.Property(u => u.PointBalance).HasDefaultValue(0);
                e.Property(u => u.Role).HasDefaultValue("USER");
                e.Property(u => u.CreatedAt).HasDefaultValueSql("SYSDATE");
                e.Property(u => u.IsActive).HasDefaultValue('Y');

                e.Property(u => u.IsActive).HasColumnType("CHAR(1)");
            });

            modelBuilder.Entity<RewardEntity>(e =>
            {
                e.Property(r => r.StockQuantity).HasDefaultValue(0);
                e.Property(r => r.CreatedAt).HasDefaultValueSql("SYSDATE");
                e.Property(r => r.IsActive).HasDefaultValue('Y');

                e.Property(r => r.IsActive).HasColumnType("CHAR(1)");
            });
        }

        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RewardRedemptionEntity> RewardRedemptions { get; set; }
        public DbSet<RewardEntity> Rewards { get; set; }
    }
}
