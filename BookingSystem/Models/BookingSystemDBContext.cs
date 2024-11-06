using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingSystem.Models
{
    public partial class BookingSystemDBContext : DbContext
    {
        public BookingSystemDBContext()
        {
        }

        public BookingSystemDBContext(DbContextOptions<BookingSystemDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClassSchedule> ClassSchedules { get; set; } = null!;
        public virtual DbSet<UserBooking> UserBookings { get; set; } = null!;
        public virtual DbSet<Waitlist> Waitlists { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.20.122.201;Database=BookingSystemDB;User Id = sa; Password = Ami123!@# ;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
