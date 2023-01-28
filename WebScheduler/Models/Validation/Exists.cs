#nullable disable

using System.ComponentModel.DataAnnotations;
using WebScheduler.Interfaces;
using WebScheduler.Repositories;

namespace WebScheduler.Models.Validation
{
    public class Exists : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Convert.ToInt32(value) != 0)
            {
                IUnitOfWork unitOfWork = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
                bool exists = unitOfWork.CountryRepository
                    .Exists(country => country.Id == Convert.ToInt32(value));
                
                if (!exists)
                {
                    return new ValidationResult("The selected option is invalid.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
