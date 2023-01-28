using MySql.EntityFrameworkCore.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebScheduler.Models
{
    [MySqlCollation("utf8_bin")]
    public class Country
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}
