using LevelUp.Application.Dtos.Redemption;
using LevelUp.Domain.Entities;

namespace LevelUp.Application.Mappers
{
    public static class RedemptionMapper
    {

        public static RedemptionResponseDto ToResponseDto(this RewardRedemptionEntity redemption)
        {
            return new RedemptionResponseDto(
                redemption.Id,
                redemption.RewardId,
                redemption.Reward.Name,
                redemption.PointsSpent,
                redemption.RedeemedAt
            );
        }
    }
}
