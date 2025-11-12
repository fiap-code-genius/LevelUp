namespace LevelUp.Application.Dtos.Redemption
{
    public record RedemptionResponseDto(
            int Id,
            int RewardId,
            string RewardName,
            int PointsSpent,
            DateTime RedeemedAt
        );
}
