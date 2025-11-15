using LevelUp.Application.Dtos.Reward;
using LevelUp.Application.Interfaces;
using LevelUp.Domain.Common;
using LevelUp.Tests.Presentation.Handlers;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace LevelUp.Tests.Presentation.Controllers
{
    public class RewardControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<IRewardUseCase> _rewardUseCaseMock;

        public RewardControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _rewardUseCaseMock = _factory.RewardUseCaseMock;
            _rewardUseCaseMock.Reset();
        }

        private RewardResponseDto CreateValidRewardResponseDto(int id = 1)
        {
            return new RewardResponseDto(id, "Gift Card", "Descrição", 100, 50);
        }

        [Fact(DisplayName = "GET /list deve retornar 200 OK com a lista de recompensas")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetAllDeveRetornar200OKComLista()
        {
            // Arrange
            var rewardList = new List<RewardResponseDto> { CreateValidRewardResponseDto(1) };
            var pageResult = new PageResultModel<IEnumerable<RewardResponseDto>>
            {
                Data = rewardList,
                Total = 1,
                Offset = 0,
                Take = 10
            };
            _rewardUseCaseMock.Setup(u => u.GetAllAsync(0, 10))
                            .ReturnsAsync(OperationResult<PageResultModel<IEnumerable<RewardResponseDto>>>.Success(pageResult));

            // Act
            var response = await _client.GetAsync("/api/v1/Reward/list");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(1, (int)content.pagination.total);
            Assert.Equal("Gift Card", (string)content.data[0].name);
        }

        [Fact(DisplayName = "GET /{id} com ID existente deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetByIdComIdExistenteDeveRetornar200OK()
        {
            // Arrange
            var rewardResponse = CreateValidRewardResponseDto(1);
            _rewardUseCaseMock.Setup(u => u.GetByIdAsync(1))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Success(rewardResponse));

            // Act
            var response = await _client.GetAsync("/api/v1/Reward/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /{id} com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetByIdComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            _rewardUseCaseMock.Setup(u => u.GetByIdAsync(99))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Failure(
                                "Recompensa não encontrada", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.GetAsync("/api/v1/Reward/99");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "POST [ADMIN] com dados válidos deve retornar 201 Created")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task CreateComDadosValidosDeveRetornar201Created()
        {
            // Arrange
            var createDto = new RewardCreateUpdateDto("Nova Recompensa", "Desc", 150, 20);
            var rewardResponse = new RewardResponseDto(5, "Nova Recompensa", "Desc", 150, 20);

            _rewardUseCaseMock.Setup(u => u.CreateAsync(It.IsAny<RewardCreateUpdateDto>()))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Success(
                                rewardResponse, (int)HttpStatusCode.Created));

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Reward", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(5, (int)content.data.id);
            Assert.Equal("Nova Recompensa", (string)content.data.name);
        }

        [Fact(DisplayName = "PUT /{id} [ADMIN] com dados válidos deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task UpdateComDadosValidosDeveRetornar200OK()
        {
            // Arrange
            var updateDto = new RewardCreateUpdateDto("Nome Atualizado", "Desc", 150, 20);
            var rewardResponse = new RewardResponseDto(1, "Nome Atualizado", "Desc", 150, 20);

            _rewardUseCaseMock.Setup(u => u.UpdateAsync(1, It.IsAny<RewardCreateUpdateDto>()))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Success(rewardResponse));

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1/Reward/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal("Nome Atualizado", (string)content.data.name);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID existente deve retornar 200 OK")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdExistenteDeveRetornar200OK()
        {
            // Arrange
            var rewardResponse = CreateValidRewardResponseDto(1);
            _rewardUseCaseMock.Setup(u => u.DeleteAsync(1))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Success(rewardResponse));

            // Act
            var response = await _client.DeleteAsync("/api/v1/Reward/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /{id} [ADMIN] com ID inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task DeleteComIdInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            _rewardUseCaseMock.Setup(u => u.DeleteAsync(99))
                            .ReturnsAsync(OperationResult<RewardResponseDto?>.Failure(
                                "Recompensa não encontrada", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.DeleteAsync("/api/v1/Reward/99");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
