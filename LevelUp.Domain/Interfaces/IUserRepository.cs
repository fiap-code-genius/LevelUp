using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;

namespace LevelUp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<PageResultModel<IEnumerable<UserEntity>>> GetAllAsync(int offset = 0, int take = 10);
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> CreateAsync(UserEntity user);
        Task<UserEntity?> UpdateAsync(int id, UserEntity user);
        Task<UserEntity?> DeleteAsync(int id);
        Task<UserEntity?> AuthenticateAsync(string email, string passwordHash);
        Task<bool> UpdateUserPointsAsync(int userId, int newPointBalance);
    }
}
