#nullable disable

using System.ComponentModel.DataAnnotations;

namespace WebScheduler.Models.Validation
{
    public class CheckboxRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((bool)value)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
