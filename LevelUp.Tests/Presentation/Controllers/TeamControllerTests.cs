using LevelUp.Application.Dtos.Team;
using LevelUp.Application.Interfaces;
using LevelUp.Domain.Common;
using LevelUp.Tests.Presentation.Handlers;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace LevelUp.Tests.Presentation.Controllers
{
    public class TeamControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<ITeamUseCase> _teamUseCaseMock;

        public TeamControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _teamUseCaseMock = _factory.TeamUseCaseMock;
            _teamUseCaseMock.Reset();
        }

        private TeamResponseDto CreateValidTeamResponseDto(int id = 1, string name = "Equipe Teste")
        {
            return new TeamResponseDto(id, name);
        }

        [Fact(DisplayName = "GET /list deve retornar 200 OK com a lista de times")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetAllDeveRetornar200OKComLista()
        {
            // Arrange
            var teamList = new List<TeamResponseDto> { CreateValidTeamResponseDto(1, "Time A") };
            var pageResult = new PageResultModel<IEnumerable<TeamResponseDto>>
            {
                Data = teamList,
                Total = 1,
                Offset = 0,
                Take = 10
            };
            _teamUseCaseMock.Setup(u => u.GetAllAsync(0, 10))
                            .ReturnsAsync(OperationResult<PageResultModel<IEnumerable<TeamResponseDto>>>.Success(pageResult));

            // Act
            var response = await _client.GetAsync("/api/v1/Team/list");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(1, (int)content.pagination.total);
            Assert.Equal("Time A", (string)content.data[0].teamName);
        }

        [Fact(DisplayName = "GET /{id} com ID existente deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetByIdComIdExistenteDeveRetornar200OK()
        {
            // Arrange
            var teamResponse = CreateValidTeamResponseDto(1);
            _teamUseCaseMock.Setup(u => u.GetByIdAsync(1))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Success(teamResponse));

            // Act
            var response = await _client.GetAsync("/api/v1/Team/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(1, (int)content.data.id);
        }

        [Fact(DisplayName = "GET /{id} com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetByIdComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            _teamUseCaseMock.Setup(u => u.GetByIdAsync(99))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Failure(
                                "Time não encontrado", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.GetAsync("/api/v1/Team/99");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "POST [ADMIN] com dados válidos deve retornar 201 Created")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task CreateComDadosValidosDeveRetornar201Created()
        {
            // Arrange
            var createDto = new TeamCreateUpdateDto("Novo Time");
            var teamResponse = CreateValidTeamResponseDto(5, "Novo Time");

            _teamUseCaseMock.Setup(u => u.CreateAsync(It.IsAny<TeamCreateUpdateDto>()))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Success(
                                teamResponse, (int)HttpStatusCode.Created));

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Team", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(5, (int)content.data.id);
            Assert.Equal("Novo Time", (string)content.data.teamName);
        }

        [Fact(DisplayName = "PUT /{id} [ADMIN] com dados válidos deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task UpdateComDadosValidosDeveRetornar200OK()
        {
            // Arrange
            var updateDto = new TeamCreateUpdateDto("Nome Atualizado");
            var teamResponse = CreateValidTeamResponseDto(1, "Nome Atualizado");

            _teamUseCaseMock.Setup(u => u.UpdateAsync(1, It.IsAny<TeamCreateUpdateDto>()))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Success(teamResponse));

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/Team/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal("Nome Atualizado", (string)content.data.teamName);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID existente deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdExistenteDeveRetornar200OK()
        {
            // Arrange
            var teamResponse = CreateValidTeamResponseDto(1);
            _teamUseCaseMock.Setup(u => u.DeleteAsync(1))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Success(teamResponse));

            // Act
            var response = await _client.DeleteAsync("/api/v1/Team/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            _teamUseCaseMock.Setup(u => u.DeleteAsync(99))
                            .ReturnsAsync(OperationResult<TeamResponseDto?>.Failure(
                                "Time não encontrado", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.DeleteAsync("/api/v1/Team/99");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
