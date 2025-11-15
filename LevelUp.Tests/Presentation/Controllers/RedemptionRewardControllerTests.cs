using LevelUp.Application.Dtos.Redemption;
using LevelUp.Application.Interfaces;
using LevelUp.Domain.Common;
using LevelUp.Tests.Presentation.Handlers;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace LevelUp.Tests.Presentation.Controllers
{
    public class RedemptionRewardControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<IRewardRedemptionUseCase> _redemptionUseCaseMock;
        private readonly int _testUserId = int.Parse(TestAuthHandler.TestUserId);

        public RedemptionRewardControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _redemptionUseCaseMock = _factory.RedemptionRewardUseCaseMock;
            _redemptionUseCaseMock.Reset();
        }

        private RedemptionResponseDto CreateValidRedemptionResponseDto(int rewardId = 5, int points = 100)
        {
            return new RedemptionResponseDto(1, rewardId, "Gift Card Teste", points, System.DateTime.UtcNow);
        }

        [Fact(DisplayName = "POST /{rewardId} com sucesso deve retornar 201 Created")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task RedeemComSucessoDeveRetornar201Created()
        {
            int rewardIdToRedeem = 5;
            var redemptionResponse = CreateValidRedemptionResponseDto(rewardIdToRedeem);

            _redemptionUseCaseMock.Setup(u => u.RedeemAsync(_testUserId, rewardIdToRedeem))
                                 .ReturnsAsync(OperationResult<RedemptionResponseDto?>.Success(
                                     redemptionResponse, (int)HttpStatusCode.Created));

            // Act
            var response = await _client.PostAsync($"/api/v1.0/RewardRedemption/{rewardIdToRedeem}", null);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(rewardIdToRedeem, (int)content.data.rewardId);
            Assert.Equal(100, (int)content.data.pointsSpent);
        }

        [Fact(DisplayName = "POST /{rewardId} com pontos insuficientes deve retornar 400 BadRequest")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task RedeemComPontosInsuficientesDeveRetornar400BadRequest()
        {
            // Arrange
            int rewardIdToRedeem = 5;
            _redemptionUseCaseMock.Setup(u => u.RedeemAsync(_testUserId, rewardIdToRedeem))
                                 .ReturnsAsync(OperationResult<RedemptionResponseDto?>.Failure(
                                     "Pontos insuficientes.", (int)HttpStatusCode.BadRequest));

            // Act
            var response = await _client.PostAsync($"/api/v1.0/RewardRedemption/{rewardIdToRedeem}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "POST /{rewardId} com recompensa inexistente deve retornar 404 NotFound")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task RedeemComRecompensaInexistenteDeveRetornar404NotFound()
        {
            // Arrange
            int rewardIdToRedeem = 99;
            _redemptionUseCaseMock.Setup(u => u.RedeemAsync(_testUserId, rewardIdToRedeem))
                                 .ReturnsAsync(OperationResult<RedemptionResponseDto?>.Failure(
                                     "Recompensa não encontrada", (int)HttpStatusCode.NotFound));

            // Act
            var response = await _client.PostAsync($"/api/v1/redemption/{rewardIdToRedeem}", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "GET /my-history deve retornar 200 OK com o histórico do usuário logado")]
        [Trait("Categoria", "Presentation - Controller")]
        public async Task GetMyHistoryDeveRetornar200OKComHistorico()
        {
            // Arrange
            var redemptionList = new List<RedemptionResponseDto> { CreateValidRedemptionResponseDto(5) };
            var pageResult = new PageResultModel<IEnumerable<RedemptionResponseDto>>
            {
                Data = redemptionList,
                Total = 1,
                Offset = 0,
                Take = 10
            };
            _redemptionUseCaseMock.Setup(u => u.GetUserRedemptionsAsync(_testUserId, 0, 10))
                                  .ReturnsAsync(OperationResult<PageResultModel<IEnumerable<RedemptionResponseDto>>>.Success(pageResult));

            // Act
            var response = await _client.GetAsync("/api/v1.0/RewardRedemption/my-history");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject(jsonString);
            Assert.Equal(1, (int)content.pagination.total);
            Assert.Equal(5, (int)content.data[0].rewardId);
        }
    }
}
