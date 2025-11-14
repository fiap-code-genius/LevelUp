using LevelUp.Application.Dtos.User;
using LevelUp.Domain.Common;

namespace LevelUp.Application.Interfaces
{
    public interface IUserUseCase
    {
        Task<OperationResult<PageResultModel<IEnumerable<UserResponseDto>>>> GetAllAsync(int offset = 0, int take = 10);
        Task<OperationResult<UserResponseDto?>> GetByIdAsync(int id);
        Task<OperationResult<UserResponseDto?>> GetByEmailAsync(string email);
        Task<OperationResult<UserResponseDto?>> UpdateAsync(int id, UserUpdateDto request);
        Task<OperationResult<UserResponseDto?>> DeleteAsync(int id);
    }
}
