using LevelUp.Application.Dtos.Redemption;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Interfaces;
using System.Net;

namespace LevelUp.Application.UseCases
{
    public class RewardRedemptionUseCase : IRewardRedemptionUseCase
    {
        private readonly IRewardRedemptionRepository _redemptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRewardRepository _rewardRepository;

        public RewardRedemptionUseCase(
                    IRewardRedemptionRepository redemptionRepository,
                    IUserRepository userRepository,
                    IRewardRepository rewardRepository)
        {
            _redemptionRepository = redemptionRepository;
            _userRepository = userRepository;
            _rewardRepository = rewardRepository;
        }

        public async Task<OperationResult<PageResultModel<IEnumerable<RedemptionResponseDto>>>> GetUserRedemptionsAsync(int userId, int offset = 0, int take = 10)
        {
            try
            {
                _ = await _userRepository.GetByIdAsync(userId);

                var pageResult = await _redemptionRepository.GetByUserIdAsync(userId, offset, take);
                var dtoList = pageResult.Data.Select(redemption => redemption.ToResponseDto());

                var responsePageResult = new PageResultModel<IEnumerable<RedemptionResponseDto>>
                {
                    Data = dtoList,
                    Total = pageResult.Total,
                    Offset = pageResult.Offset,
                    Take = pageResult.Take
                };

                return OperationResult<PageResultModel<IEnumerable<RedemptionResponseDto>>>.Success(responsePageResult);
            }
            catch (Exception ex)
            {
                return OperationResult<PageResultModel<IEnumerable<RedemptionResponseDto>>>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<RedemptionResponseDto?>> RedeemAsync(int userId, int rewardId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var reward = await _rewardRepository.GetByIdAsync(rewardId);

                if (user.PointBalance < reward.PointCost)
                {
                    return OperationResult<RedemptionResponseDto?>.Failure("Pontos insuficientes.", (int)HttpStatusCode.BadRequest);
                }

                if (reward.StockQuantity <= 0)
                {
                    return OperationResult<RedemptionResponseDto?>.Failure("Recompensa fora de estoque.", (int)HttpStatusCode.BadRequest);
                }

                int newPointBalance = user.PointBalance - reward.PointCost;

                await _userRepository.UpdateUserPointsAsync(userId, newPointBalance);

                reward.StockQuantity -= 1;
                await _rewardRepository.UpdateAsync(rewardId, reward);

                var redemption = new RewardRedemptionEntity
                {
                    UserId = userId,
                    RewardId = rewardId,
                    PointsSpent = reward.PointCost,
                    RedeemedAt = DateTime.UtcNow
                };

                var newRedemption = await _redemptionRepository.CreateAsync(redemption);

                var responseDto = new RedemptionResponseDto(
                    newRedemption.Id,
                    newRedemption.RewardId,
                    reward.Name,
                    newRedemption.PointsSpent,
                    newRedemption.RedeemedAt
                );

                return OperationResult<RedemptionResponseDto?>.Success(responseDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return OperationResult<RedemptionResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
