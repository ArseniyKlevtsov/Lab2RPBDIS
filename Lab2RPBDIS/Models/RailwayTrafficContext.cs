using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lab2RPBDIS.Models;

public partial class RailwayTrafficContext : DbContext
{
    public RailwayTrafficContext()
    {
    }

    public RailwayTrafficContext(DbContextOptions<RailwayTrafficContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Stop> Stops { get; set; }

    public virtual DbSet<Train> Trains { get; set; }

    public virtual DbSet<TrainInformation> TrainInformations { get; set; }

    public virtual DbSet<TrainStaff> TrainStaffs { get; set; }

    public virtual DbSet<TrainType> TrainTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.;Database=RailwayTraffic;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF135E37B70");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeeName).HasMaxLength(50);
            entity.Property(e => e.HireDate).HasColumnType("date");
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK_Employees_Positions");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59BDCC2B5F");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69ED01C16A");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.StopId).HasColumnName("StopID");
            entity.Property(e => e.TrainId).HasColumnName("TrainID");

            entity.HasOne(d => d.Stop).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.StopId)
                .HasConstraintName("FK_Schedule_Stops");

            entity.HasOne(d => d.Train).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TrainId)
                .HasConstraintName("FK_Schedule_Trains");
        });

        modelBuilder.Entity<Stop>(entity =>
        {
            entity.HasKey(e => e.StopId).HasName("PK__Stops__EB6A38D411600A97");

            entity.Property(e => e.StopId).HasColumnName("StopID");
            entity.Property(e => e.StopName).HasMaxLength(50);
        });

        modelBuilder.Entity<Train>(entity =>
        {
            entity.HasKey(e => e.TrainId).HasName("PK__Trains__8ED2725A3C7158A1");

            entity.Property(e => e.TrainId).HasColumnName("TrainID");
            entity.Property(e => e.ArrivalStopId).HasColumnName("ArrivalStopID");
            entity.Property(e => e.DepartureStopId).HasColumnName("DepartureStopID");
            entity.Property(e => e.TrainNumber).HasMaxLength(4);
            entity.Property(e => e.TrainTypeId).HasColumnName("TrainTypeID");

            entity.HasOne(d => d.ArrivalStop).WithMany(p => p.TrainArrivalStops)
                .HasForeignKey(d => d.ArrivalStopId)
                .HasConstraintName("FK_Trains_ArrivalStops");

            entity.HasOne(d => d.DepartureStop).WithMany(p => p.TrainDepartureStops)
                .HasForeignKey(d => d.DepartureStopId)
                .HasConstraintName("FK_Trains_DepartureStops");

            entity.HasOne(d => d.TrainType).WithMany(p => p.Trains)
                .HasForeignKey(d => d.TrainTypeId)
                .HasConstraintName("FK_Trains_TrainTypes");
        });

        modelBuilder.Entity<TrainInformation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TrainInformation");

            entity.Property(e => e.ArrivalStop).HasMaxLength(50);
            entity.Property(e => e.DepartureStop).HasMaxLength(50);
            entity.Property(e => e.IsBrandedTrain)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.TrainId).HasColumnName("TrainID");
            entity.Property(e => e.TrainNumber).HasMaxLength(4);
            entity.Property(e => e.TrainType).HasMaxLength(50);
        });

        modelBuilder.Entity<TrainStaff>(entity =>
        {
            entity.HasKey(e => e.TrainStaffId).HasName("PK__TrainSta__AFB59FE656D6C936");

            entity.Property(e => e.TrainStaffId).HasColumnName("TrainStaffID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.TrainId).HasColumnName("TrainID");

            entity.HasOne(d => d.Employee).WithMany(p => p.TrainStaffs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_TrainStaffs_Employees");

            entity.HasOne(d => d.Train).WithMany(p => p.TrainStaffs)
                .HasForeignKey(d => d.TrainId)
                .HasConstraintName("FK_TrainStaffs_Trains");
        });

        modelBuilder.Entity<TrainType>(entity =>
        {
            entity.HasKey(e => e.TrainTypeId).HasName("PK__TrainTyp__487216540C835691");

            entity.Property(e => e.TrainTypeId).HasColumnName("TrainTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
