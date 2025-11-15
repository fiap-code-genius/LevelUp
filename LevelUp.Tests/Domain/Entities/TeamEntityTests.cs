using LevelUp.Domain.Entities;

namespace LevelUp.Tests.Domain.Entities
{
    public class TeamEntityTests
    {
        private TeamEntity CreateValidTeam()
        {
            return new TeamEntity
            {
                Id = 1,
                TeamName = "Equipe de Teste Válida"
            };
        }

        [Fact(DisplayName = "TeamEntity com dados válidos deve passar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void TeamEntityComDadosValidosDevePassarNaValidacao()
        {
            // Arrange
            var team = CreateValidTeam();

            // Act
            var validationResults = ValidationHelper.Validate(team);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact(DisplayName = "TeamEntity sem TeamName deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void TeamEntitySemTeamNameDeveFalharNaValidacao()
        {
            // Arrange
            var team = CreateValidTeam();
            team.TeamName = string.Empty;

            // Act
            var validationResults = ValidationHelper.Validate(team);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("TeamName") &&
                v.ErrorMessage.Contains("required")
            );
        }

        [Fact(DisplayName = "TeamEntity com TeamName muito longo deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void TeamEntityComTeamNameMuitoLongoDeveFalharNaValidacao()
        {
            // Arrange
            var team = CreateValidTeam();
            team.TeamName = new string('A', 101);

            // Act
            var validationResults = ValidationHelper.Validate(team);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("TeamName") &&
                v.ErrorMessage.Contains("length")
            );
        }
    }
}
