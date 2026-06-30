using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class LifeFlowDbContext : DbContext
{
    public LifeFlowDbContext()
    {
    }

    public LifeFlowDbContext(DbContextOptions<LifeFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BloodStock> BloodStocks { get; set; }

    public virtual DbSet<DemandForecast> DemandForecasts { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<ShortageAlert> ShortageAlerts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BloodStock>(entity =>
        {
            entity.HasKey(e => e.BloodType).HasName("PK__BloodSto__33141D1732A50600");

            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Normal");
            entity.Property(e => e.SupplyLevel).HasDefaultValue(100);
        });

        modelBuilder.Entity<DemandForecast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DemandFo__3214EC078386CAE0");

            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ForecastDate).HasColumnType("datetime");
            entity.Property(e => e.Hospital)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Donation__3214EC07CDE81EC9");

            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Contact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DonationDate).HasColumnType("datetime");
            entity.Property(e => e.DonorName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Hospital)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
        });

        modelBuilder.Entity<ShortageAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shortage__3214EC075C1D0225");

            entity.Property(e => e.BloodType)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Hospital)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC079DE5AD97");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Users_PhoneNumber").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Client", "DF_Users_Role_Client");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
