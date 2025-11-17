using LevelUp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.AppData
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RewardEntity> Rewards { get; set; }
        public DbSet<RewardRedemptionEntity> RewardRedemptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(e =>
            {
                e.Property(u => u.PointBalance).HasDefaultValue(0);
                e.Property(u => u.Role).HasDefaultValue("USER");
                e.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                e.Property(u => u.IsActive).HasDefaultValue('Y').HasColumnType("CHAR(1)");

                e.HasOne(user => user.Team)
                 .WithMany(team => team.Users)
                 .HasForeignKey(user => user.TeamId)
                 .HasConstraintName("FK_USERS_TEAM");
            });

            modelBuilder.Entity<RewardEntity>(e =>
            {
                e.Property(r => r.StockQuantity).HasDefaultValue(0);
                e.Property(r => r.CreatedAt).HasDefaultValueSql("GETDATE()");
                e.Property(r => r.IsActive).HasDefaultValue('Y').HasColumnType("CHAR(1)");
            });

            modelBuilder.Entity<RewardRedemptionEntity>(e =>
            {
                e.HasOne(redemption => redemption.User)
                 .WithMany(user => user.RewardRedemptions)
                 .HasForeignKey(redemption => redemption.UserId)
                 .HasConstraintName("FK_REDEMPTIONS_USER");

                e.HasOne(redemption => redemption.Reward)
                 .WithMany(reward => reward.RewardRedemptions)
                 .HasForeignKey(redemption => redemption.RewardId)
                 .HasConstraintName("FK_REDEMPTIONS_REWARD");
            });
        }
    }
}
