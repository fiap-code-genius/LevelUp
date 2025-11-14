using LevelUp.Application.Dtos.Redemption;
using LevelUp.Domain.Common;
using Swashbuckle.AspNetCore.Filters;

namespace LevelUp.Doc.Samples.Redemption
{
    public class RedemptionListResponseSample : IExamplesProvider<PageResultModel<IEnumerable<RedemptionResponseDto>>>
    {
        public PageResultModel<IEnumerable<RedemptionResponseDto>> GetExamples()
        {
            var redemptionList = new List<RedemptionResponseDto>
            {
                new RedemptionResponseDto(
                    Id: 1,
                    RewardId: 5,
                    RewardName: "Gift Card iFood R$100",
                    PointsSpent: 600,
                    RedeemedAt: DateTime.UtcNow.AddDays(-2)
                ),
                new RedemptionResponseDto(
                    Id: 2,
                    RewardId: 7,
                    RewardName: "Créditos na Alura",
                    PointsSpent: 100,
                    RedeemedAt: DateTime.UtcNow.AddDays(-15)
                )
            };

            return new PageResultModel<IEnumerable<RedemptionResponseDto>>
            {
                Data = redemptionList,
                Offset = 0,
                Take = 10,
                Total = 2
            };
        }
    }
}
