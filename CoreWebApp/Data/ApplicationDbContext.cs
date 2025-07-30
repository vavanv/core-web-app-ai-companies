using Microsoft.EntityFrameworkCore;
using CoreWebApp.Models;

namespace CoreWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Chatbot> Chatbots { get; set; }
        public DbSet<LLM> LLMs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.LastLoginAt);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Create unique index on Email
                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });

            // Configure Company entity
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Create unique index on Name
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            // Configure Chatbot entity
            modelBuilder.Entity<Chatbot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Company)
                    .WithMany(e => e.Chatbots)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure LLM entity
            modelBuilder.Entity<LLM>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Specialization)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Company)
                    .WithMany(e => e.LLMs)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}