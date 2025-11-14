using LevelUp.Application.Dtos.Auth;
using LevelUp.Application.Dtos.User;
using LevelUp.Application.Interfaces;
using LevelUp.Doc.Samples.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LevelUp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUseCase _authUseCase;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthUseCase authUseCase, IConfiguration configuration)
        {
            _authUseCase = authUseCase;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Autentica um usuário",
            Description = "Valida as credenciais e retorna um Token JWT se o login for bem-sucedido."
        )]
        [SwaggerRequestExample(typeof(AuthRequestDto), typeof(AuthRequestSample))]
        [SwaggerResponse(statusCode: 200, description: "Login bem-sucedido", type: typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Requisição inválida")]
        [SwaggerResponse(statusCode: 401, description: "Credenciais inválidas")]
        [SwaggerResponse(statusCode: 500, description: "Erro interno do servidor")]
        [SwaggerResponseExample(statusCode: 200, typeof(AuthResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
        {
            var result = await _authUseCase.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var token = GenerateJwtToken(result.Value!.User);

            var response = new AuthResponseDto(
                Token: token,
                User: result.Value.User
            );

            var hateoas = new
            {
                data = response,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action(nameof(Login), "Auth", null, Request.Scheme) },
                    new { rel = "register", href = Url.Action(nameof(Register), "Auth", null, Request.Scheme) },
                    new { rel = "get_user_profile", href = Url.Action("GetSelf", "User", null, Request.Scheme) }
                }
            };
            
            return Ok(hateoas);
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Registra um novo usuário",
            Description = "Cria uma nova conta de usuário com as informações fornecidas."
        )]
        [SwaggerRequestExample(typeof(UserCreateDto), typeof(UserCreateRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Usuário registrado com sucesso", type: typeof(UserResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Dados inválidos para registro")]
        [SwaggerResponse(statusCode: 409, description: "O e-mail informado já está em uso")]
        [SwaggerResponse(statusCode: 500, description: "Erro interno do servidor")]
        [SwaggerResponseExample(statusCode: 201, typeof(UserResponseSample))]
        [EnableRateLimiting("ratelimit")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto request)
        {
            var result = await _authUseCase.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            var hateoasResponse = new
            {
                data = result.Value,
                links = new object[]
                {
                    new { rel = "self", href = Url.Action("GetById", "User", new { id = result.Value!.Id }, Request.Scheme) },
                    new { rel = "login", href = Url.Action(nameof(Login), "Auth", null, Request.Scheme) }
                }
            };

            return StatusCode(result.StatusCode, hateoasResponse);
        }

        private string GenerateJwtToken(UserResponseDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString()),
                    new Claim(ClaimTypes.Email, dto.Email),
                    new Claim(ClaimTypes.Name, dto.FullName),
                    new Claim(ClaimTypes.Role, dto.Role),
                }),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiresInHours"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
