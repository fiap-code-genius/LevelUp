using System.ComponentModel.DataAnnotations;

namespace LevelUp.Tests.Domain
{
    public static class ValidationHelper
    {
        public static IList<ValidationResult> Validate(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);

            Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

            return validationResults;
        }
    }
}
