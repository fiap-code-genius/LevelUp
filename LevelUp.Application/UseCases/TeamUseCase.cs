using LevelUp.Application.Dtos.Team;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Errors;
using LevelUp.Domain.Interfaces;
using System.Net;

namespace LevelUp.Application.UseCases
{
    public class TeamUseCase : ITeamUseCase
    {
        private readonly ITeamRepository _repository;
        public TeamUseCase(ITeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<TeamResponseDto?>> CreateAsync(TeamCreateUpdateDto request)
        {
            try
            {
                var newTeam = await _repository.CreateAsync(request.ToEntity());
                return OperationResult<TeamResponseDto?>.Success(newTeam?.ToResponseDto(), (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return OperationResult<TeamResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<TeamResponseDto?>> DeleteAsync(int id)
        {
            try
            {
                var deletedTeam = await _repository.DeleteAsync(id);
                return OperationResult<TeamResponseDto?>.Success(deletedTeam?.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<TeamResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<TeamResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<PageResultModel<IEnumerable<TeamResponseDto>>>> GetAllAsync(int offset = 0, int take = 10)
        {
            try
            {
                var pageResult = await _repository.GetAllAsync(offset, take);
                var dtoList = pageResult.Data.Select(team => team.ToResponseDto());

                var responsePageResult = new PageResultModel<IEnumerable<TeamResponseDto>>
                {
                    Data = dtoList,
                    Total = pageResult.Total,
                    Offset = pageResult.Offset,
                    Take = pageResult.Take
                };

                return OperationResult<PageResultModel<IEnumerable<TeamResponseDto>>>.Success(responsePageResult);
            }
            catch (Exception ex)
            {
                return OperationResult<PageResultModel<IEnumerable<TeamResponseDto>>>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<TeamResponseDto?>> GetByIdAsync(int id)
        {
            try
            {
                var team = await _repository.GetByIdAsync(id);
                return OperationResult<TeamResponseDto?>.Success(team?.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<TeamResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<TeamResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<TeamResponseDto?>> UpdateAsync(int id, TeamCreateUpdateDto request)
        {
            try
            {
                var updatedTeam = await _repository.UpdateAsync(id, request.ToEntity());

                if (updatedTeam == null)
                {
                    throw new IdNotFoundException($"Time com ID: {id} - não encontrado para atualizar.");
                }

                return OperationResult<TeamResponseDto?>.Success(updatedTeam.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<TeamResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<TeamResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
