#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySql.EntityFrameworkCore.Extensions;
using WebScheduler.Models;

namespace WebScheduler.Database
{
    public class DataContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>()
                .ForMySQLHasCollation("utf8_bin");
            modelBuilder.Entity<IdentityUserRole<string>>()
                .ForMySQLHasCollation("utf8_bin");
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .ForMySQLHasCollation("utf8_bin");
            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .ForMySQLHasCollation("utf8_bin");

            modelBuilder.Ignore<IdentityUserLogin<string>>();
            modelBuilder.Ignore<IdentityUserToken<string>>();

            modelBuilder.Entity<User>()
                .Ignore(user => user.AccessFailedCount)
                .Ignore(user => user.ConcurrencyStamp)
                .Ignore(user => user.SecurityStamp)
                .Ignore(user => user.PhoneNumber)
                .Ignore(user => user.PhoneNumberConfirmed)
                .Ignore(user => user.TwoFactorEnabled);

            modelBuilder.Entity<User>()
                .ToTable("Users");
            modelBuilder.Entity<IdentityRole>()
                .ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>()
                .ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .ToTable("UserClaims");
            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .ToTable("RoleClaims");

            modelBuilder.Entity<User>()
                .Property(user => user.UserName)
                .HasColumnName("Username")
                .HasColumnType("varchar(32)");

            modelBuilder.Entity<User>()
               .Property(user => user.Email)
               .HasColumnType("varchar(320)");

            modelBuilder.Entity<User>()
               .Property(user => user.PasswordHash)
               .HasColumnName("Password")
               .HasColumnType("varchar(255)");

            modelBuilder.Entity<IdentityRole>()
                .Ignore(role => role.ConcurrencyStamp);

            modelBuilder.Entity<User>()
                .HasOne(user => user.Country)
                .WithMany(country => country.Users)
                .HasForeignKey(user => user.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Appointments)
                .WithOne(app => app.User)
                .HasForeignKey(user => user.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Country>()
                .HasData(new Country[]
                {
                    new Country(){ Id = 1, Name = "Croatia" },
                    new Country(){ Id = 2, Name = "Austria" },
                    new Country(){ Id = 3, Name = "Germany" },
                    new Country(){ Id = 4, Name = "France" },
                    new Country(){ Id = 5, Name = "Spain" },
                    new Country(){ Id = 6, Name = "Switzerland" },
                    new Country(){ Id = 7, Name = "Italy" },
                    new Country(){ Id = 8, Name = "Slovenia" },
                    new Country(){ Id = 9, Name = "Romania" },
                    new Country(){ Id = 10, Name = "Poland" },
                });
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Appointment> Appointments{ get; set; }
    }

    public class MysqlEntityFrameworkDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkMySQL();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                .TryAddCoreServices();
        }
    }
}
