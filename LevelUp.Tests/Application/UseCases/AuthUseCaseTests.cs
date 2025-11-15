using LevelUp.Application.Dtos.Auth;
using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Application.UseCases;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Errors;
using LevelUp.Domain.Interfaces;
using Moq;
using System.Net;

namespace LevelUp.Tests.Application.UseCases
{
    public class AuthUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IAuthUseCase _authUseCase;

        private const string TestPassword = "Password123!";
        private readonly string TestPasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword);

        public AuthUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authUseCase = new AuthUseCase(_userRepositoryMock.Object);
        }

        private UserEntity CreateValidUserEntity(int id = 1, string email = "teste@levelup.com")
        {
            return new UserEntity
            {
                Id = id,
                FullName = "Usuário de Teste",
                Email = email,
                PasswordHash = TestPasswordHash,
                Role = "USER",
                IsActive = 'Y',
                PointBalance = 100
            };
        }

        private UserCreateDto CreateValidRegisterDto(string email = "novo@levelup.com")
        {
            return new UserCreateDto(
                "Novo Usuário",
                email,
                "NovaSenha456!",
                "Tester Jr.",
                1
            );
        }

        [Fact(DisplayName = "LoginAsync com credenciais válidas deve retornar Success")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task LoginAsyncComCredenciaisValidasDeveRetornarSuccess()
        {
            // Arrange
            var user = CreateValidUserEntity();
            var loginDto = new AuthRequestDto(user.Email, TestPassword);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
                               .ReturnsAsync(user);

            // Act
            var result = await _authUseCase.LoginAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(user.Email, result.Value.User.Email);
            Assert.Equal(user.Id, result.Value.User.Id);
        }

        [Fact(DisplayName = "LoginAsync com senha incorreta deve retornar Failure 401")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task LoginAsyncComSenhaIncorretaDeveRetornarFailure401()
        {
            // Arrange
            var user = CreateValidUserEntity();
            var loginDto = new AuthRequestDto(user.Email, "SENHA-ERRADA");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
                               .ReturnsAsync(user);

            // Act
            var result = await _authUseCase.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Contains("Email ou senha inválidos", result.Error);
        }

        [Fact(DisplayName = "LoginAsync com email inexistente deve retornar Failure 401")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task LoginAsyncComEmailInexistenteDeveRetornarFailure401()
        {
            // Arrange
            var loginDto = new AuthRequestDto("naoexiste@levelup.com", "qualquer-senha");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email))
                               .ThrowsAsync(new EmailNotFoundException("Usuário não encontrado"));

            // Act
            var result = await _authUseCase.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Contains("Email ou senha inválidos", result.Error);
        }

        [Fact(DisplayName = "RegisterAsync com email novo deve retornar Success 201")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task RegisterAsyncComEmailNovoDeveRetornarSuccess201()
        {
            // Arrange
            var registerDto = CreateValidRegisterDto("novo@levelup.com");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(registerDto.Email))
                               .ThrowsAsync(new EmailNotFoundException("Email não encontrado"));
            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserEntity>()))
                               .ReturnsAsync((UserEntity user) =>
                               {
                                   user.Id = 99;
                                   return user;
                               });

            // Act
            var result = await _authUseCase.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(99, result.Value.Id);
            Assert.Equal(registerDto.Email, result.Value.Email);
            _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
        }

        [Fact(DisplayName = "RegisterAsync deve hashear a senha corretamente")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task RegisterAsyncDeveHashearASenhaCorretamente()
        {
            // Arrange
            var registerDto = CreateValidRegisterDto("hash@levelup.com");
            UserEntity userSalvo = null;

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(registerDto.Email))
                               .ThrowsAsync(new EmailNotFoundException("Email não encontrado"));

            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserEntity>()))
                               .Callback<UserEntity>(user => userSalvo = user)
                               .ReturnsAsync((UserEntity user) => user);

            // Act
            await _authUseCase.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(userSalvo);
            Assert.NotEqual(registerDto.Password, userSalvo.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify(registerDto.Password, userSalvo.PasswordHash));
        }

        [Fact(DisplayName = "RegisterAsync com email duplicado deve retornar Failure 409")]
        [Trait("Categoria", "Application - UseCase")]
        public async Task RegisterAsyncComEmailDuplicadoDeveRetornarFailure409()
        {
            // Arrange
            var existingUser = CreateValidUserEntity(1, "existente@levelup.com");
            var registerDto = CreateValidRegisterDto("existente@levelup.com");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(registerDto.Email))
                               .ReturnsAsync(existingUser);

            // Act
            var result = await _authUseCase.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.Contains("Este e-mail já está em uso", result.Error);
            _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Never);
        }
    }
}
