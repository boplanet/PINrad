using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PINrad.Models;

namespace PINrad.Data
{
    public class ApplicationDbContext : IdentityDbContext<RegLogUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet za zaduženja
        public DbSet<Assignment> Assignments { get; set; }

        // DbSet za imovinu
        public DbSet<Asset> Assets { get; set; }

        // DbSet za posebne korisnike (CustomUser)
        public DbSet<CustomUser> CustomUsers { get; set; }

        // Konfiguracija modela
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija relacije između Asset i CustomUser
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.AssignedUser)
                .WithMany(u => u.AssignedAssets)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.SetNull); // Ako se korisnik obriše, UserID će biti null

            // Configure InventarniBroj as unique
            modelBuilder.Entity<Asset>()
                .Property(a => a.InventarniBroj)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.InventarniBroj)
                .IsUnique(); // Ensures uniqueness


            modelBuilder.Entity<Asset>()
                .Property(a => a.KupovnaVrijednost)
                .HasColumnType("decimal(18,2)"); // Preciznost 18, skala 2 (do dva decimalna mjesta)

            modelBuilder.Entity<Asset>()
                .Property(a => a.Opis)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CustomUser>()
                .Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CustomUser>()
                .Property(u => u.Department)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CustomUser>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            // Specifično postavljanje duljine za UserId u AspNetUserClaims na nvarchar(450)
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .Property(uc => uc.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            // Specifično postavljanje duljine za UserId u AspNetUserLogins na nvarchar(450)
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .Property(ul => ul.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            // Specifično postavljanje duljine za UserId u AspNetUserRoles na nvarchar(450)
            modelBuilder.Entity<IdentityUserRole<string>>()
                .Property(ur => ur.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            // Specifično postavljanje duljine za UserId u AspNetUserTokens na nvarchar(450)
            modelBuilder.Entity<IdentityUserToken<string>>()
                .Property(ut => ut.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            /*// Postavi default duljinu za sve string kolone na nvarchar(250)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.Name == "UserID")
                    {
                        property.SetColumnType("nvarchar(450)");
                    }
                }
            }*/

            //  Identity tablice pravilno konfiguriran
            base.OnModelCreating(modelBuilder);


        }
    }
}
