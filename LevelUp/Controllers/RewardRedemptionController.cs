using Asp.Versioning;
using LevelUp.Application.Dtos.Redemption;
using LevelUp.Application.Interfaces;
using LevelUp.Doc.Samples.Redemption;
using LevelUp.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Security.Claims;

namespace LevelUp.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "USER, ADMIN")]
    public class RewardRedemptionController : ControllerBase
    {
        private readonly IRewardRedemptionUseCase _redemptionUseCase;

        public RewardRedemptionController(IRewardRedemptionUseCase redemptionUseCase)
        {
            _redemptionUseCase = redemptionUseCase;
        }

        [HttpPost("{rewardId}")]
        [SwaggerOperation(
            Summary = "Resgata uma recompensa",
            Description = "Tenta resgatar uma recompensa para o usuário autenticado, debitando os pontos do seu saldo."
        )]
        [SwaggerResponse(statusCode: 201, description: "Recompensa resgatada com sucesso", typeof(RedemptionResponseDto))]
        [SwaggerResponse(statusCode: 500, description: "Pontos insuficientes ou recompensa fora de estoque")]
        [SwaggerResponse(statusCode: 404, description: "Recompensa ou usuário não encontrado")]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 201, typeof(RedemptionResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Redeem(int rewardId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new { error = "Token inválido ou não contém ID do usuário." });
            }

            var result = await _redemptionUseCase.RedeemAsync(userId.Value, rewardId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetMyHistory), "RedemptionReward", null, Request.Scheme) },
                    new { rel = "view_reward", href = Url.Action("GetById", "Reward", new { id = result.Value.RewardId }, Request.Scheme) }
                }
            };

            return StatusCode(result.StatusCode, hateoasResponse);
        }

        [HttpGet("my-history")]
        [SwaggerOperation(
            Summary = "Busca o histórico de resgates (extrato)",
            Description = "Retorna uma lista paginada de todas as recompensas que o usuário autenticado já resgatou."
        )]
        [SwaggerResponse(statusCode: 200, description: "Histórico de resgates", typeof(PageResultModel<IEnumerable<RedemptionResponseDto>>))]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RedemptionListResponseSample))]
        public async Task<IActionResult> GetMyHistory([FromQuery] int offset = 0, [FromQuery] int take = 10)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new { error = "Token inválido ou não contém ID do usuário." });
            }

            var result = await _redemptionUseCase.GetUserRedemptionsAsync(userId.Value, offset, take);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value.Data,
                pagination = new
                {
                    result.Value.Offset,
                    result.Value.Take,
                    result.Value.Total
                },
                links = new object[]
                {
                     new { rel = "self", href = Url.Action(nameof(GetMyHistory), "RedemptionReward", new { offset, take }, Request.Scheme) },
                     new { rel = "view_store", href = Url.Action("GetAll", "Reward", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
