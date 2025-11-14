using LevelUp.Application.Dtos.Reward;
using LevelUp.Domain.Common;

namespace LevelUp.Application.Interfaces
{
    public interface IRewardUseCase
    {
        Task<OperationResult<PageResultModel<IEnumerable<RewardResponseDto>>>> GetAllAsync(int offset = 0, int take = 10);
        Task<OperationResult<RewardResponseDto?>> GetByIdAsync(int id);
        Task<OperationResult<RewardResponseDto?>> CreateAsync(RewardCreateUpdateDto request);
        Task<OperationResult<RewardResponseDto?>> UpdateAsync(int id, RewardCreateUpdateDto request);
        Task<OperationResult<RewardResponseDto?>> DeleteAsync(int id);
    }
}
