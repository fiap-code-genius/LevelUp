using Asp.Versioning;
using LevelUp.Application.Dtos.Team;
using LevelUp.Application.Interfaces;
using LevelUp.Doc.Samples.Team;
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
    public class TeamController : ControllerBase
    {
        private readonly ITeamUseCase _teamUseCase;

        public TeamController(ITeamUseCase teamUseCase)
        {
            _teamUseCase = teamUseCase;
        }

        [HttpGet("list")]
        [Authorize(Roles = "USER, ADMIN")]
        [SwaggerOperation(
            Summary = "Lista todos os times",
            Description = "Retorna uma lista paginada de todos os times disponíveis."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de times", typeof(PageResultModel<IEnumerable<TeamResponseDto>>))]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 200, typeof(TeamListResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int take = 10)
        {
            var result = await _teamUseCase.GetAllAsync(offset, take);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value.Data.Select(team => new
                {
                    team.Id,
                    team.TeamName,
                    links = new object[]
                    {
                        new { rel = "self", href = Url.Action(nameof(GetById), "Team", new { id = team.Id }, Request.Scheme) },
                        new { rel = "update", href = Url.Action(nameof(Update), "Team", new { id = team.Id }, Request.Scheme) },
                        new { rel = "delete", href = Url.Action(nameof(Delete), "Team", new { id = team.Id }, Request.Scheme) }
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
                     new { rel = "self", href = Url.Action(nameof(GetAll), "Team", new { offset, take }, Request.Scheme) },
                     new { rel = "create", href = Url.Action(nameof(Create), "Team", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "USER, ADMIN")]
        [SwaggerOperation(
            Summary = "Busca time por ID",
            Description = "Retorna os dados de um time específico."
        )]
        [SwaggerResponse(statusCode: 200, description: "Dados do time", typeof(TeamResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Time não encontrado")]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponseExample(statusCode: 200, typeof(TeamResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _teamUseCase.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Team", new { id = id }, Request.Scheme) },
                    new { rel = "update", href = Url.Action(nameof(Update), "Team", new { id = id }, Request.Scheme) },
                    new { rel = "delete", href = Url.Action(nameof(Delete), "Team", new { id = id }, Request.Scheme) },
                    new { rel = "get_all", href = Url.Action(nameof(GetAll), "Team", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Cria um novo time",
            Description = "Cria um novo time no sistema."
        )]
        [SwaggerRequestExample(typeof(TeamCreateUpdateDto), typeof(TeamCreateRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Time criado com sucesso", typeof(TeamResponseDto))]
        [SwaggerResponse(statusCode: 500, description: "Dados inválidos")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 201, typeof(TeamResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Create([FromBody] TeamCreateUpdateDto request)
        {
            var result = await _teamUseCase.CreateAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Team", new { id = result.Value.Id }, Request.Scheme) },
                    new { rel = "update", href = Url.Action(nameof(Update), "Team", new { id = result.Value.Id }, Request.Scheme) },
                    new { rel = "delete", href = Url.Action(nameof(Delete), "Team", new { id = result.Value.Id }, Request.Scheme) }
                }
            };

            return StatusCode(result.StatusCode, hateoasResponse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Atualiza um time",
            Description = "Atualiza o nome de um time existente."
        )]
        [SwaggerRequestExample(typeof(TeamCreateUpdateDto), typeof(TeamUpdateRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Time atualizado", typeof(TeamResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Time não encontrado")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 201, typeof(TeamResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Update(int id, [FromBody] TeamCreateUpdateDto request)
        {
            var result = await _teamUseCase.UpdateAsync(id, request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "Team", new { id = id }, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Deleta um time",
            Description = "Deleta um time permanentemente da base dados"
        )]
        [SwaggerResponse(statusCode: 200, description: "Time deletado", typeof(TeamResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Time não encontrado")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(TeamResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _teamUseCase.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                     new { rel = "get_all", href = Url.Action(nameof(GetAll), "Team", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }
    }
}
