using Asp.Versioning;
using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Doc.Samples.User;
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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;

        public UserController(IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        [HttpGet("me")]
        [Authorize(Roles = "ADMIN, USER")]
        [SwaggerOperation(
            Summary = "Busca o perfil do usuário logado",
            Description = "Retorna os dados do perfil do usuário que está autenticado via Token JWT."
        )]
        [SwaggerResponse(statusCode: 200, description: "Perfil do usuário", typeof(UserResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponse(statusCode: 404, "Usuário do token não encontrado")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(UserResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetSelf()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, new { error = "Token inválido ou não contém ID do usuário." });
            }

            var result = await _userUseCase.GetByIdAsync(userId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetSelf), "User", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }


        [HttpGet("list")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Lista todos os usuários",
            Description = "Retorna uma lista paginada de todos os usuários no sistema."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de usuários", typeof(PageResultModel<IEnumerable<UserResponseDto>>))]
        [SwaggerResponse(statusCode: 401, description: "Não autenticado")]
        [SwaggerResponse(statusCode: 403, description: "Não autorizado (requer role ADMIN)")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserListResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int take = 10)
        {
            var result = await _userUseCase.GetAllAsync(offset, take);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value.Data.Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.JobTitle,
                    user.PointBalance,
                    user.Role,
                    links = new object[]
                    {
                        new { rel = "self", href = Url.Action(nameof(GetById), "User", new { id = user.Id }, Request.Scheme) },
                        new { rel = "update", href = Url.Action(nameof(Update), "User", new { id = user.Id }, Request.Scheme) },
                        new { rel = "delete", href = Url.Action(nameof(Delete), "User", new { id = user.Id }, Request.Scheme) }
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
                     new { rel = "self", href = Url.Action(nameof(GetAll), "User", new { offset, take }, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Busca usuário por ID",
            Description = "Retorna os dados de um usuário específico."
        )]
        [SwaggerResponse(statusCode: 200, description: "Dados do usuário", typeof(UserResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [SwaggerResponse(statusCode: 401, description:"Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userUseCase.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "User", new { id = id }, Request.Scheme) },
                    new { rel = "update", href = Url.Action(nameof(Update), "User", new { id = id }, Request.Scheme) },
                    new { rel = "delete", href = Url.Action(nameof(Delete), "User", new { id = id }, Request.Scheme) },
                    new { rel = "get_all", href = Url.Action(nameof(GetAll), "User", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Atualiza um usuário",
            Description = "Atualiza dados cadastrais de um usuário (não atualiza senha)."
        )]
        [SwaggerRequestExample(typeof(UserUpdateDto), typeof(UserUpdateRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Usuário atualizado", typeof(UserResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [SwaggerResponse(statusCode: 401, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 201, typeof(UserResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto request)
        {
            var result = await _userUseCase.UpdateAsync(id, request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(GetById), "User", new { id = id }, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "[ADMIN] Desativa (Soft Delete) um usuário",
            Description = "Marca um usuário como Inativo (IS_ACTIVE = 'N'). O usuário não é fisicamente deletado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Usuário desativado", typeof(UserResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [SwaggerResponse(statusCode: 401, description: "Não autorizado")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userUseCase.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                     new { rel = "get_all", href = Url.Action(nameof(GetAll), "User", null, Request.Scheme) }
                }
            };

            return Ok(hateoasResponse);
        }
    }
}
