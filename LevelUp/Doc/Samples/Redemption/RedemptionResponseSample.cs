using LevelUp.Application.Dtos.Redemption;
using Swashbuckle.AspNetCore.Filters;

namespace LevelUp.Doc.Samples.Redemption
{
    public class RedemptionResponseSample : IExamplesProvider<RedemptionResponseDto>
    {
        public RedemptionResponseDto GetExamples()
        {
            return new RedemptionResponseDto(
                Id: 1,
                RewardId: 5,
                RewardName: "Gift Card iFood R$100",
                PointsSpent: 600,
                RedeemedAt: DateTime.UtcNow.AddMinutes(-5)
            );
        }
    }
}
