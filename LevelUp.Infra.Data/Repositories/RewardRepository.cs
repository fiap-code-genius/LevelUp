using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Errors;
using LevelUp.Infra.Data.AppData;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly ApplicationContext _context;
        public RewardRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<RewardEntity?> CreateAsync(RewardEntity reward)
        {
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task<RewardEntity?> DeleteAsync(int id)
        {
            var reward = await GetByIdAsync(id);

            if (reward is null)
            {
                throw new IdNotFoundException($"Recompensa com ID: {id} - não encontrada para deletar.");
            }

            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task<PageResultModel<IEnumerable<RewardEntity>>> GetAllAsync(int offset = 0, int take = 10)
        {
            var total = await _context.Rewards.CountAsync();

            var data = await _context.Rewards
                .OrderBy(r => r.PointCost)
                .Skip(offset)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();

            return new PageResultModel<IEnumerable<RewardEntity>>
            {
                Data = data,
                Total = total,
                Offset = offset,
                Take = take
            };
        }

        public async Task<RewardEntity?> GetByIdAsync(int id)
        {
            var reward = await _context.Rewards
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reward is null)
            {
                throw new NoContentException("Usuário não encontrado.");
            }

            return reward;
        }

        public async Task<RewardEntity?> UpdateAsync(int id, RewardEntity reward)
        {
            var existingReward = await _context.Rewards.FindAsync(id);
            if (existingReward != null)
            {
                existingReward.Name = reward.Name;
                existingReward.Description = reward.Description;
                existingReward.PointCost = reward.PointCost;
                existingReward.StockQuantity = reward.StockQuantity;

                _context.Rewards.Update(existingReward);
                await _context.SaveChangesAsync();
                return existingReward;
            }
            return null;
        }
    }
}
