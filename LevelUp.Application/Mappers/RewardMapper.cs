using LevelUp.Application.Dtos.Reward;
using LevelUp.Domain.Entities;

namespace LevelUp.Application.Mappers
{
    public static class RewardMapper
    {

        public static RewardEntity ToEntity(this RewardCreateUpdateDto dto)
        {
            return new RewardEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                PointCost = dto.PointCost,
                StockQuantity = dto.StockQuantity
            };
        }

        public static RewardResponseDto ToResponseDto(this RewardEntity reward)
        {
            return new RewardResponseDto(
                            reward.Id,
                            reward.Name,
                            reward.Description,
                            reward.PointCost,
                            reward.StockQuantity
                        );
        }
    }
}
