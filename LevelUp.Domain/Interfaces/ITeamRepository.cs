using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;

namespace LevelUp.Domain.Interfaces
{
    public interface ITeamRepository
    {
        Task<PageResultModel<IEnumerable<TeamEntity>>> GetAllAsync(int offset = 0, int take = 10);
        Task<TeamEntity?> GetByIdAsync(int id);
        Task<TeamEntity?> CreateAsync(TeamEntity team);
        Task<TeamEntity?> UpdateAsync(int id, TeamEntity team);
        Task<TeamEntity?> DeleteAsync(int id);
    }
}
