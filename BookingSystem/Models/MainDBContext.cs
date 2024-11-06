using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingSystem.Models
{
    public partial class MainDBContext : DbContext
    {
        IConfiguration Configuration;
        public MainDBContext()
        {
            var builder = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public MainDBContext(DbContextOptions<MainDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Package> Packages { get; set; } = null!;
        public virtual DbSet<UserPackage> UserPackages { get; set; } = null!;

        public virtual DbSet<ClassSchedule> ClassSchedules { get; set; } = null!;
        public virtual DbSet<UserBooking> UserBookings { get; set; } = null!;
        public virtual DbSet<Waitlist> Waitlists { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DBConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Credits).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.ToTable("Package");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Country)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<UserPackage>(entity =>
            {
                entity.ToTable("UserPackage");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClassSchedule>(entity =>
            {
                entity.ToTable("ClassSchedule");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserBooking>(entity =>
            {
                entity.ToTable("UserBooking");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BookingTime).HasColumnType("datetime");

                entity.HasOne(d => d.ClassSchedule)
                    .WithMany(p => p.UserBookings)
                    .HasForeignKey(d => d.ClassScheduleId)
                    .HasConstraintName("FK__UserBooki__Class__403A8C7D");
            });

            modelBuilder.Entity<Waitlist>(entity =>
            {
                entity.ToTable("Waitlist");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.WaitlistTime).HasColumnType("datetime");

                entity.HasOne(d => d.ClassSchedule)
                    .WithMany(p => p.Waitlists)
                    .HasForeignKey(d => d.ClassScheduleId)
                    .HasConstraintName("FK__Waitlist__ClassS__440B1D61");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
