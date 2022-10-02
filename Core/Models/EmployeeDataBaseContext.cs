using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Models
{
    public partial class EmployeeDataBaseContext : DbContext
    {
        public EmployeeDataBaseContext()
        {
        }

        public EmployeeDataBaseContext(DbContextOptions<EmployeeDataBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PasswordHash).HasColumnType("image");

                entity.Property(e => e.PasswordSalt).HasColumnType("image");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.RefreshToken).HasMaxLength(100);

                entity.Property(e => e.Salary).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.TokenCreated).HasColumnType("datetime");

                entity.Property(e => e.TokenExpires).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
