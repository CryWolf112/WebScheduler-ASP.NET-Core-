#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebScheduler.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property |
    AttributeTargets.Field, AllowMultiple = false)]
    sealed public class DateBetween : ValidationAttribute
    {
        public string StartDate { get; set; }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                DateTime dateTime = Convert.ToDateTime(value);
                DateTime startDate = DateTime.Parse(StartDate).Date;

                if (dateTime < startDate || dateTime > DateTime.UtcNow)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
