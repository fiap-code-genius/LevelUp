using LevelUp.Application.Dtos.Team;
using LevelUp.Domain.Common;

namespace LevelUp.Application.Interfaces
{
    public interface ITeamUseCase
    {
        Task<OperationResult<PageResultModel<IEnumerable<TeamResponseDto>>>> GetAllAsync(int offset = 0, int take = 10);
        Task<OperationResult<TeamResponseDto?>> GetByIdAsync(int id);
        Task<OperationResult<TeamResponseDto?>> CreateAsync(TeamCreateUpdateDto request);
        Task<OperationResult<TeamResponseDto?>> UpdateAsync(int id, TeamCreateUpdateDto request);
        Task<OperationResult<TeamResponseDto?>> DeleteAsync(int id);
    }
}
