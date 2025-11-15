using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Domain.Common;
using LevelUp.Tests.Presentation.Handlers;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace LevelUp.Tests.Presentation.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<IUserUseCase> _userUseCaseMock;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _userUseCaseMock = _factory.UserUseCaseMock;

            _userUseCaseMock.Reset();
        }

        private UserResponseDto CreateValidUserResponseDto(int id = 1, string role = "USER")
        {
            return new UserResponseDto(id, "Usuário Teste", "teste@levelup.com", "Tester", 100, role, 1);
        }

        [Fact(DisplayName = "GET /me deve retornar 200 OK e o perfil do usuário logado")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetSelfDeveRetornar200OKParaUsuarioLogado()
        {
            // Arrange
            var userResponse = CreateValidUserResponseDto();

            _userUseCaseMock.Setup(u => u.GetByIdAsync(1))
                            .ReturnsAsync(OperationResult<UserResponseDto?>.Success(userResponse));

            // Act
            var response = await _client.GetAsync("/api/v1/User/me");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(userResponse.Email, (string)content.data.email);
        }

        [Fact(DisplayName = "GET /list [ADMIN] deve retornar 200 OK com a lista paginada")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetAllDeveRetornar200OKComLista()
        {
            // Arrange
            var userList = new List<UserResponseDto> { CreateValidUserResponseDto(1) };
            var pageResult = new PageResultModel<IEnumerable<UserResponseDto>>
            {
                Data = userList,
                Total = 1,
                Offset = 0,
                Take = 10
            };
            _userUseCaseMock.Setup(u => u.GetAllAsync(0, 10))
                            .ReturnsAsync(OperationResult<PageResultModel<IEnumerable<UserResponseDto>>>.Success(pageResult));

            // Act
            var response = await _client.GetAsync("/api/v1/User/list");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(1, (int)content.pagination.total);
            Assert.Equal("Usuário Teste", (string)content.data[0].fullName);
        }

        [Fact(DisplayName = "PUT /{id} [ADMIN] com dados válidos deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task UpdateComDadosValidosDeveRetornar200OK()
        {
            // Arrange
            var updateDto = new UserUpdateDto("Nome Mudado", "email@novo.com", "QA Pleno", "USER", 1, 500);
            var userResponse = CreateValidUserResponseDto(1);
            userResponse = userResponse with { FullName = updateDto.FullName };

            _userUseCaseMock.Setup(u => u.UpdateAsync(1, It.IsAny<UserUpdateDto>()))
                            .ReturnsAsync(OperationResult<UserResponseDto?>.Success(userResponse));

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/User/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal("Nome Mudado", (string)content.data.fullName);
        }

        [Fact(DisplayName = "PUT /{id} [ADMIN] com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task UpdateComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            var updateDto = new UserUpdateDto("Nome", "email@novo.com", "QA", "USER", 1, 500);
            _userUseCaseMock.Setup(u => u.UpdateAsync(99, It.IsAny<UserUpdateDto>()))
                            .ReturnsAsync(OperationResult<UserResponseDto?>.Failure(
                                "Usuário não encontrado", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/User/99", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID existente deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdExistenteDeveRetornar200OK()
        {
            // Arrange
            var userResponse = CreateValidUserResponseDto(1);
            _userUseCaseMock.Setup(u => u.DeleteAsync(1))
                            .ReturnsAsync(OperationResult<UserResponseDto?>.Success(userResponse));

            // Act
            var response = await _client.DeleteAsync("/api/v1/User/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            _userUseCaseMock.Setup(u => u.DeleteAsync(99))
                            .ReturnsAsync(OperationResult<UserResponseDto?>.Failure(
                                "Usuário não encontrado", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.DeleteAsync("/api/v1/User/99");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
