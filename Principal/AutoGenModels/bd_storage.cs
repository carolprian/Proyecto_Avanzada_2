﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoGens;

public partial class bd_storage : DbContext
{
    public bd_storage()
    {
    }

    public bd_storage(DbContextOptions<bd_storage> options)
        : base(options)
    {
    }

    public virtual DbSet<Academy> Academies { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<Coordinator> Coordinators { get; set; }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<DyLequipment> DyLequipments { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Maintain> Maintain { get; set; }

    public virtual DbSet<MaintenanceRegister> MaintenanceRegisters { get; set; }

    public virtual DbSet<MaintenanceType> MaintenanceTypes { get; set; }

    public virtual DbSet<Petition> Petitions { get; set; }

    public virtual DbSet<PetitionDetail> PetitionDetails { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestDetail> RequestDetails { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Storer> Storers { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teach> Teaches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        string path = Path.Combine(Environment.CurrentDirectory, "bd_storage.db");
        string connection = $"Filename={path}";
        ConsoleColor backgoundColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"Connection : {connection}");
        ForegroundColor = backgoundColor;
        // Use The DB Motor
        optionsBuilder.UseSqlite(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
