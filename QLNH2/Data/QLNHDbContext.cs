using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLNH2.Models;

namespace QLNH2.Data;

public partial class QLNHDbContext : DbContext
{
    public QLNHDbContext(DbContextOptions<QLNHDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Hocsinh> Hocsinhs { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameClass)
                .HasMaxLength(100)
                .HasColumnName("name_class");
            entity.Property(e => e.NameSubject)
                .HasMaxLength(100)
                .HasColumnName("name_subject");
            entity.Property(e => e.Schoolid).HasColumnName("schoolid");
            entity.Property(e => e.TimeCreate).HasColumnName("time_create");
            entity.Property(e => e.TimeUpdate).HasColumnName("time_update");

            entity.HasOne(d => d.School).WithMany(p => p.Classes)
                .HasForeignKey(d => d.Schoolid)
                .HasConstraintName("FK_Class_School");
        });

        modelBuilder.Entity<Hocsinh>(entity =>
        {
            entity.ToTable("Hocsinh");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Classid).HasColumnName("classid");
            entity.Property(e => e.CodeStudent)
                .HasMaxLength(20)
                .HasColumnName("code_student");
            entity.Property(e => e.NameStudent)
                .HasMaxLength(100)
                .HasColumnName("name_student");
            entity.Property(e => e.TimeCreate).HasColumnName("time_create");
            entity.Property(e => e.TimeUpdate).HasColumnName("time_update");

            entity.HasOne(d => d.Class).WithMany(p => p.Hocsinhs)
                .HasForeignKey(d => d.Classid)
                .HasConstraintName("FK_Hocsinh_Class");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.ToTable("School");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.NameSchool)
                .HasMaxLength(200)
                .HasColumnName("name_school");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.TimeCreate).HasColumnName("time_create");
            entity.Property(e => e.TimeUpdate).HasColumnName("time_update");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
