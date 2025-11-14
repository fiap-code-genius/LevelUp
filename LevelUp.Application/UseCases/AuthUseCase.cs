using LevelUp.Application.Dtos.Auth;
using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Application.Mappers;
using LevelUp.Domain.Common;
using LevelUp.Domain.Errors;
using LevelUp.Domain.Interfaces;
using System.Net;

namespace LevelUp.Application.UseCases
{
    public class AuthUseCase : IAuthUseCase
    {
        private readonly IUserRepository _userRepository;

        public AuthUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<OperationResult<AuthResponseDto?>> LoginAsync(AuthRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user?.PasswordHash);

                if (!isPasswordValid)
                {
                    throw new EmailNotFoundException($"Usuário com Email: {request.Email} - Não encontrado.");
                }

                var userDto = user?.ToResponseDto();

                var response = new AuthResponseDto("PlaceHolder", userDto);

                return OperationResult<AuthResponseDto?>.Success(response);
            }
            catch(Exception ex) when (ex is EmailNotFoundException || ex is NoContentException)
            {
                return OperationResult<AuthResponseDto?>.Failure("Email ou senha inválidos.", (int)HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                return OperationResult<AuthResponseDto?>.Failure($"Erro interno: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<UserResponseDto?>> RegisterAsync(UserCreateDto request)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return OperationResult<UserResponseDto?>.Failure("Este e-mail já está em uso.", (int)HttpStatusCode.Conflict);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var userEntity = request.ToEntity(hashedPassword);

                var newUser = await _userRepository.CreateAsync(userEntity);

                var responseDto = newUser?.ToResponseDto();
                return OperationResult<UserResponseDto?>.Success(responseDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return OperationResult<UserResponseDto?>.Failure($"Erro interno: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
