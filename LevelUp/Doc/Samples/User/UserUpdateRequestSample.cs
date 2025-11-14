using LevelUp.Application.Dtos.User;
using Swashbuckle.AspNetCore.Filters;

namespace LevelUp.Doc.Samples.User
{
    public class UserUpdateRequestSample : IExamplesProvider<UserUpdateDto>
    {
        public UserUpdateDto GetExamples()
        {
            return new UserUpdateDto(
                FullName: "Usuário Atualizado",
                Email: "usuario.atualizado@levelup.com",
                JobTitle: "Analista de QA Pleno",
                Role: "USER",
                TeamId: 2,
                PointBalance: 1500
            );
        }
    }
}
