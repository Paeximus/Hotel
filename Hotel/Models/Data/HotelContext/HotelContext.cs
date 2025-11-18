using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models.Data.HotelContext;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EHYEH-ASHER-EHY\\SQLEXPRESS;Database=Hotel; Integrated Security = true; TrustServerCertificate = true; MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ConplaintId)
                .HasName("PK8")
                .IsClustered(false);

            entity.ToTable("Complaint");

            entity.Property(e => e.DateOfComplaint).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");

            entity.HasOne(d => d.Room).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRoom9");

            entity.HasOne(d => d.User).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsers10");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorId)
                .HasName("PK10")
                .IsClustered(false);
            entity.Property(e => e.FloorName).HasMaxLength(50);

            entity.ToTable("Floor");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId)
                .HasName("PK3")
                .IsClustered(false);

            entity.ToTable("Image");

            entity.Property(e => e.Image1).HasColumnName("Image");

            entity.HasOne(d => d.Room).WithMany(p => p.Images)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRoom1");
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.PrivilegeId)
                .HasName("PK7")
                .IsClustered(false);

            entity.ToTable("Privilege");

            entity.Property(e => e.Description).HasColumnType("text");

            entity.HasOne(d => d.Room).WithMany(p => p.Privileges)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRoom4");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId)
                .HasName("PK5")
                .IsClustered(false);

            entity.ToTable("Reservation");

            entity.Property(e => e.ArrivalDate).HasColumnType("date");
            entity.Property(e => e.CarRegNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DateOfExit).HasColumnType("date");
            entity.Property(e => e.ModeOfOrder).HasMaxLength(10);
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.WithCar)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRoom6");
            entity.HasOne(d => d.User).WithMany(p => p.Resevations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefUsers5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId)
                .HasName("PK9")
                .IsClustered(false);

            entity.Property(e => e.RoleId).HasMaxLength(1);
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId)
                .HasName("PK2")
                .IsClustered(false);

            entity.ToTable("Room");

            entity.Property(e => e.IsOccupied)
                .HasMaxLength(10)
                .HasColumnName("isOccupied");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoomNo).HasMaxLength(10);
            entity.Property(e => e.RoomImage).HasColumnName("RoomImage");

            entity.HasOne(d => d.Floor).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefFloor15");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PK4")
                .IsClustered(false);

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.OtherNames).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefRoles12");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
