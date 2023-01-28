using Microsoft.AspNetCore.Mvc;
using MySql.EntityFrameworkCore.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace WebScheduler.Models
{
    [MySqlCollation("utf8_bin")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType("varchar(32)")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime? EndsAt { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }
    }
}
