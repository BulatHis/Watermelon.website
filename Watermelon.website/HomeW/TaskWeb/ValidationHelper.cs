using System.ComponentModel.DataAnnotations;

namespace Watermelon.website;

public static class ValidationHelper
{
    public static List<ValidationResult> ValidateModel<T>(T model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        if (!Validator.TryValidateObject(model, context, results, true))
        {
            return results;
        }
        else
        {
            return null;
        }
    }
}