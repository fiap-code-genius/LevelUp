using Asp.Versioning;
using LevelUp.Application.Dtos.Reward;
using LevelUp.Application.Interfaces;
using LevelUp.Doc.Samples.Reward;
using LevelUp.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace LevelUp.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class RewardController : Controller
    {
        private readonly IRewardUseCase _rewardUseCase;

        public RewardController(IRewardUseCase rewardUseCase)
        {
            _rewardUseCase = rewardUseCase;
        }

        [HttpGet("list")]
        [Authorize(Roles = "USER, ADMIN")]
        [SwaggerOperation(
            Summary = "Lista todas as recompensas (Loja)",
            Description = "Retorna uma lista paginada de todas as recompensas disponíveis para resgate."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de recompensas", typeof(PageResultModel<IEnumerable<RewardResponseDto>>))]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RewardListResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int take = 10)
        {
            var result = await _rewardUseCase.GetAllAsync(offset, take);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value.Data.Select(reward => new
                {
                    reward.Id,
                    reward.Name,
                    reward.Description,
                    reward.PointCost,
                    reward.StockQuantity,
                    links = new object[]
                    {
                        new { rel = "self", href = Url.Action(nameof(GetById), "Reward", new { id = reward.Id }, Request.Scheme) },
                        new { rel = "redeem", href = Url.Action("Redeem", "RedemptionReward", new { rewardId = reward.Id }, Request.Scheme) }
                    }
                }),
                pagination = new
                {
                    result.Value.Offset,
                    result.Value.Take,
                    result.Value.Total
                },
                links = new object[]
                {
                     new { rel = "self", href = Url.Action(nameof(GetAll), "Reward", new { offset, take }, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "USER, ADMIN")]
        [SwaggerOperation(
            Summary = "Busca recompensa por ID",
            Description = "Retorna os dados de uma recompensa específica."
        )]
        [SwaggerResponse(statusCode: 200, description: "Dados da recompensa", typeof(RewardResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Recompensa não encontrada")]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RewardResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _rewardUseCase.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Reward", new { id = id }, Request.Scheme) },
                    new { rel = "redeem", href = Url.Action("Redeem", "RedemptionReward", new { rewardId = id }, Request.Scheme) },
                    new { rel = "get_all", href = Url.Action(nameof(GetAll), "Reward", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Cria uma nova recompensa",
            Description = "Adiciona um novo item na loja de recompensas."
        )]
        [SwaggerRequestExample(typeof(RewardCreateUpdateDto), typeof(RewardCreateRequestSample))]
        [SwaggerResponse(statusCode: 200, description: "Recompensa criada com sucesso", typeof(RewardResponseDto))]
        [SwaggerResponse(statusCode: 500, description: "Dados inválidos")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RewardResponseSample))]
        public async Task<IActionResult> Create([FromBody] RewardCreateUpdateDto request)
        {
            var result = await _rewardUseCase.CreateAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Reward", new { id = result.Value.Id }, Request.Scheme) }
                }
            };

            return StatusCode(result.StatusCode, hateoasResponse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Atualiza uma recompensa",
            Description = "Atualiza os dados (nome, custo, estoque) de uma recompensa existente."
        )]
        [SwaggerRequestExample(typeof(RewardCreateUpdateDto), typeof(RewardUpdateRequestSample))]
        [SwaggerResponse(statusCode: 200, description: "Recompensa atualizada", typeof(RewardResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Recompensa não encontrada")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RewardResponseSample))]
        public async Task<IActionResult> Update(int id, [FromBody] RewardCreateUpdateDto request)
        {
            var result = await _rewardUseCase.UpdateAsync(id, request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Reward", new { id = id }, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
                    Summary = "[ADMIN] Desativa (Soft Delete) uma recompensa",
                    Description = "Marca uma recompensa como Inativa (IS_ACTIVE = 'N')."
                )]
        [SwaggerResponse(statusCode: 200, description: "Recompensa desativada", typeof(RewardResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Recompensa não encontrada")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(RewardResponseSample))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rewardUseCase.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                     new { rel = "get_all", href = Url.Action(nameof(GetAll), "Reward", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }
    }
}
