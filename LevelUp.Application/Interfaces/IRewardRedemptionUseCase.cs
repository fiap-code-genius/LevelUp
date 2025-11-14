using LevelUp.Application.Dtos.Redemption;
using LevelUp.Domain.Common;

namespace LevelUp.Application.Interfaces
{
    public interface IRewardRedemptionUseCase
    {
        Task<OperationResult<RedemptionResponseDto?>> RedeemAsync(int userId, int rewardId);
        Task<OperationResult<PageResultModel<IEnumerable<RedemptionResponseDto>>>> GetUserRedemptionsAsync(int userId, int offset = 0, int take = 10);
    }
}
