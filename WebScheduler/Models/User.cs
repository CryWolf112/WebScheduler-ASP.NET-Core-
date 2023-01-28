using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySql.EntityFrameworkCore.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using WebScheduler.Models.Validation;

namespace WebScheduler.Models
{
    [MySqlCollation("utf8_bin")]
    public class User : IdentityUser
    {
        [Column("FirstName", TypeName = "varchar(32)")]
        [StringLength(32, ErrorMessage = "The {0} must not have more than {1} items.")]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "The {0} format is invalid.")]
        [Display(Name = "first name")]
        public string? FirstName { get; set; }

        [Column("LastName", TypeName = "varchar(32)")]
        [StringLength(32, ErrorMessage = "The {0} must not have more than {1} items.")]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "The {0} format is invalid.")]
        [Display(Name = "last name")]
        public string? LastName { get; set; }

        [Column("BirthDate", TypeName = "date")]
        [DateBetween(StartDate = "1900-01-01", ErrorMessage = "The selected {0} is invalid.")]
        [Display(Name = "birth date")]
        public DateTime? BirthDate { get; set; }

        [Column("Gender", TypeName = "varchar(6)")]
        [RegularExpression("^((male)|(female))$", ErrorMessage = "The selected {0} is invalid.")]
        [Display(Name = "gender")]
        public string? Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        [RegularExpression("^[A-Za-z0-9]*$", ErrorMessage = "The {0} format is invalid.")]
        [IsUnique(ColumnName = "username")]
        [Display(Name = "username")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage = "The {0} format is invalid.")]
        [IsUnique(ColumnName = "email")]
        [Display(Name = "email")]
        public override string Email { get => base.Email; set => base.Email = value; }

        [Column("DateCreated", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get;set; }

        [Column("CountryId")]
        [Exists()]
        [Display(Name = "country_ID")]
        public int? CountryId { get; set; }

        public Country? Country { get; set; }

        public List<Appointment>? Appointments { get; set; }
    }
}
