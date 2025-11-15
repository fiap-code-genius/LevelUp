using LevelUp.Domain.Entities;

namespace LevelUp.Tests.Domain.Entities
{
    public class UserEntityTests
    {
        private UserEntity CreateValidUser()
        {
            return new UserEntity
            {
                Id = 1,
                FullName = "Usuário de Teste Válido",
                Email = "teste@valido.com",
                PasswordHash = "abc.123.xyz.hash",
                JobTitle = "Analista de Testes",
                PointBalance = 100,
                Role = "USER",
                CreatedAt = DateTime.UtcNow,
                IsActive = 'Y'
            };
        }

        [Fact(DisplayName = "UserEntity com dados válidos deve passar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntityComDadosValidosDevePassarNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact(DisplayName = "UserEntity sem FullName deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntitySemFullNameDeveFalharNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();
            user.FullName = string.Empty;

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("FullName") &&
                v.ErrorMessage.Contains("required")
            );
        }

        [Fact(DisplayName = "UserEntity com FullName muito longo deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntityComFullNameMuitoLongoDeveFalharNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();
            user.FullName = new string('A', 256);

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("FullName") &&
                v.ErrorMessage.Contains("length")
            );
        }

        [Fact(DisplayName = "UserEntity com Email inválido deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntityComEmailInvalidoDeveFalharNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();
            user.Email = "email-invalido.com";

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("Email") &&
                v.ErrorMessage.Contains("formato válido.")
            );
        }

        [Fact(DisplayName = "UserEntity sem PasswordHash deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntitySemPasswordHashDeveFalharNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();
            user.PasswordHash = null;

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("PasswordHash") &&
                v.ErrorMessage.Contains("required")
            );
        }

        [Fact(DisplayName = "UserEntity sem Role deve falhar na validação")]
        [Trait("Categoria", "Domain - Entity")]
        public void UserEntitySemRoleDeveFalharNaValidacao()
        {
            // Arrange
            var user = CreateValidUser();
            user.Role = null;

            // Act
            var validationResults = ValidationHelper.Validate(user);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v =>
                v.MemberNames.Contains("Role") &&
                v.ErrorMessage.Contains("required")
            );
        }
    }
}
