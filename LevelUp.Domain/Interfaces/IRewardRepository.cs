using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;

namespace LevelUp.Domain.Interfaces
{
    public interface IRewardRepository
    {
        Task<PageResultModel<IEnumerable<RewardEntity>>> GetAllAsync(int offset = 0, int take = 10);
        Task<RewardEntity?> GetByIdAsync(int id);
        Task<RewardEntity?> CreateAsync(RewardEntity reward);
        Task<RewardEntity?> UpdateAsync(int id, RewardEntity reward);
        Task<RewardEntity?> DeleteAsync(int id);
    }
}
