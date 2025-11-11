using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Infra.Data.AppData;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.Repositories
{
    public class RewardRedemptionRepository : IRewardRedemptionRepository
    {
        private readonly ApplicationContext _context;
        public RewardRedemptionRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<RewardRedemptionEntity?> CreateAsync(RewardRedemptionEntity redemption)
        {
            _context.RewardRedemptions.Add(redemption);
            await _context.SaveChangesAsync();
            return redemption;
        }

        public async Task<PageResultModel<IEnumerable<RewardRedemptionEntity>>> GetByUserIdAsync(int userId, int offset = 0, int take = 10)
        {
            var query = _context.RewardRedemptions
                .Where(r => r.UserId == userId);

            var total = await query.CountAsync();

            var data = await query
                .Include(r => r.Reward)
                .OrderByDescending(r => r.RedeemedAt)
                .Skip(offset)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();

            return new PageResultModel<IEnumerable<RewardRedemptionEntity>>
            {
                Data = data,
                Total = total,
                Offset = offset,
                Take = take
            };
        }
    }
}
