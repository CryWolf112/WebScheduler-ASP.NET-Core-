#nullable disable

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebScheduler.Database;
using WebScheduler.Interfaces;

namespace WebScheduler.Models.Validation
{
    public class IsUnique : ValidationAttribute
    {
        public string ColumnName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                IUnitOfWork unitOfWork = (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));

                bool exists;

                if (ColumnName == "username")
                {
                    exists = unitOfWork.UserRepository
                       .Exists(user => user.UserName == value.ToString());
                }
                else
                {
                    exists = unitOfWork.UserRepository
                        .Exists(user => user.Email == value.ToString());
                }

                if (exists)
                {
                    return new ValidationResult(string.Format("The {0} is alredy registrated.", ColumnName));
                }

                return ValidationResult.Success;
            }

            return new ValidationResult(string.Format("The {0} filed is required", ColumnName));

        }
    }
}
