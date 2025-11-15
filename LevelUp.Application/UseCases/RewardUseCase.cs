using LevelUp.Application.Dtos.Reward;
using LevelUp.Application.Dtos.Team;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Errors;
using LevelUp.Domain.Interfaces;
using System.Net;

namespace LevelUp.Application.UseCases
{
    public class RewardUseCase : IRewardUseCase
    {
        private readonly IRewardRepository _repository;

        public RewardUseCase(IRewardRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<RewardResponseDto?>> CreateAsync(RewardCreateUpdateDto request)
        {
            try
            {
                var newReward = await _repository.CreateAsync(request.ToEntity());
                return OperationResult<RewardResponseDto?>.Success(newReward?.ToResponseDto(), (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return OperationResult<RewardResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<RewardResponseDto?>> DeleteAsync(int id)
        {
            try
            {
                var deletedReward = await _repository.DeleteAsync(id);
                return OperationResult<RewardResponseDto?>.Success(deletedReward?.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<RewardResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<RewardResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<PageResultModel<IEnumerable<RewardResponseDto>>>> GetAllAsync(int offset = 0, int take = 10)
        {
            try
            {
                var pageResult = await _repository.GetAllAsync(offset, take);
                var dtoList = pageResult.Data.Select(reward => reward.ToResponseDto());

                var responsePageResult = new PageResultModel<IEnumerable<RewardResponseDto>>
                {
                    Data = dtoList,
                    Total = pageResult.Total,
                    Offset = pageResult.Offset,
                    Take = pageResult.Take
                };

                return OperationResult<PageResultModel<IEnumerable<RewardResponseDto>>>.Success(responsePageResult);
            }
            catch (Exception ex)
            {
                return OperationResult<PageResultModel<IEnumerable<RewardResponseDto>>>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<RewardResponseDto?>> GetByIdAsync(int id)
        {
            try
            {
                var reward = await _repository.GetByIdAsync(id);
                return OperationResult<RewardResponseDto?>.Success(reward.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<RewardResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<RewardResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<RewardResponseDto?>> UpdateAsync(int id, RewardCreateUpdateDto request)
        {
            try
            {
                var updatedReward = await _repository.UpdateAsync(id, request.ToEntity());

                if (updatedReward == null)
                {
                    throw new IdNotFoundException($"Recompensa com ID: {id} - não encontrada para atualizar.");
                }

                return OperationResult<RewardResponseDto?>.Success(updatedReward.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<RewardResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<RewardResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
