using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Interfaces;
using System.Net;

namespace LevelUp.Application.UseCases
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _repository;

        public UserUseCase(IUserRepository repository)
        {
            _repository = repository;
        }
        public Task<OperationResult<UserResponseDto?>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<PageResultModel<IEnumerable<UserResponseDto>>>> GetAllAsync(int offset = 0, int take = 10)
        {
            try
            {
                var pageResult = await _repository.GetAllAsync(offset, take);

                var dtoList = pageResult.Data.Select(user => user.ToResponseDto());

                var responsePageResult = new PageResultModel<IEnumerable<UserResponseDto>>
                {
                    Data = dtoList,
                    Total = pageResult.Total,
                    Offset = pageResult.Offset,
                    Take = pageResult.Take
                };

                return OperationResult<PageResultModel<IEnumerable<UserResponseDto>>>.Success(responsePageResult);
            }
            catch (Exception ex)
            {
                return OperationResult<PageResultModel<IEnumerable<UserResponseDto>>>.Failure(
                    $"Erro interno ao buscar usuários: {ex.Message}",
                    (int)HttpStatusCode.InternalServerError
                );
            }
        }

        public Task<OperationResult<UserResponseDto?>> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<UserResponseDto?>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<UserResponseDto?>> UpdateAsync(int id, UserUpdateDto request)
        {
            throw new NotImplementedException();
        }
    }
}
