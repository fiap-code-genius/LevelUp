using LevelUp.Application.Dtos.Team;
using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Errors;
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
        public async Task<OperationResult<UserResponseDto?>> DeleteAsync(int id)
        {
            try
            {
                var deletedUser = await _repository.DeleteAsync(id);

                return OperationResult<UserResponseDto?>.Success(deletedUser?.ToResponseDto());
            }
            catch (Exception ex)
            {
                return OperationResult<UserResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
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

        public async Task<OperationResult<UserResponseDto?>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _repository.GetByEmailAsync(email);
                return OperationResult<UserResponseDto?>.Success(user?.ToResponseDto());
            }
            catch (Exception ex)
            {
                return OperationResult<UserResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<UserResponseDto?>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);
                return OperationResult<UserResponseDto?>.Success(user?.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<UserResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<UserResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }

        public async Task<OperationResult<UserResponseDto?>> UpdateAsync(int id, UserUpdateDto request)
        {
            try
            {
                var userEntityUpdate = new UserEntity
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    JobTitle = request.JobTitle,
                    Role = request.Role,
                    TeamId = request.TeamId,
                    PasswordHash = "",

                    PointBalance = request.PointBalance ?? -1
                };

                var updatedUser = await _repository.UpdateAsync(id, userEntityUpdate);

                if (updatedUser == null)
                {
                    throw new IdNotFoundException($"Usuário com ID: {id} - não encontrado para atualizar.");
                }

                return OperationResult<UserResponseDto?>.Success(updatedUser.ToResponseDto());
            }
            catch (IdNotFoundException ex)
            {
                return OperationResult<UserResponseDto?>.Failure(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return OperationResult<UserResponseDto?>.Failure($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
