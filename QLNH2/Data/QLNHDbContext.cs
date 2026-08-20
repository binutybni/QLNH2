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

    public virtual DbSet<Dkmh> Dkmhs { get; set; }

    public virtual DbSet<Gvgd> Gvgds { get; set; }

    public virtual DbSet<Hocsinh> Hocsinhs { get; set; }

    public virtual DbSet<Nh> Nhs { get; set; }

    public virtual DbSet<Pcgvgd> Pcgvgds { get; set; }

    public virtual DbSet<PointStudent> PointStudents { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameClass)
                .HasMaxLength(100)
                .HasColumnName("name_class");
            entity.Property(e => e.Schoolid).HasColumnName("schoolid");
            entity.Property(e => e.TimeCreate).HasColumnName("time_create");
            entity.Property(e => e.TimeUpdate).HasColumnName("time_update");

            entity.HasOne(d => d.School).WithMany(p => p.Classes)
                .HasForeignKey(d => d.Schoolid)
                .HasConstraintName("FK_Class_School");
        });

        modelBuilder.Entity<Dkmh>(entity =>
        {
            entity.ToTable("DKMH");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdGvgdmh).HasColumnName("idGVGDMH");
            entity.Property(e => e.IdHs).HasColumnName("idHS");
            entity.Property(e => e.TimeRegister).HasColumnName("time_register");

            entity.HasOne(d => d.IdGvgdmhNavigation).WithMany(p => p.Dkmhs)
                .HasForeignKey(d => d.IdGvgdmh)
                .HasConstraintName("FK_DKMH_PCGVGD");

            entity.HasOne(d => d.IdHsNavigation).WithMany(p => p.Dkmhs)
                .HasForeignKey(d => d.IdHs)
                .HasConstraintName("FK_DKMH_Hocsinh");
        });

        modelBuilder.Entity<Gvgd>(entity =>
        {
            entity.ToTable("GVGD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaGvgd)
                .HasMaxLength(50)
                .HasColumnName("MaGVGD");
            entity.Property(e => e.TenGvgd)
                .HasMaxLength(200)
                .HasColumnName("TenGVGD");
            entity.Property(e => e.TimeCre).HasColumnName("time_cre");
            entity.Property(e => e.TimeUp).HasColumnName("time_up");
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

        modelBuilder.Entity<Nh>(entity =>
        {
            entity.ToTable("NH");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaNh)
                .HasMaxLength(50)
                .HasColumnName("MaNH");
            entity.Property(e => e.TenNh)
                .HasMaxLength(200)
                .HasColumnName("TenNH");
            entity.Property(e => e.TimeCre).HasColumnName("time_cre");
            entity.Property(e => e.TimeUp).HasColumnName("time_up");
        });

        modelBuilder.Entity<Pcgvgd>(entity =>
        {
            entity.ToTable("PCGVGD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdClass).HasColumnName("idClass");
            entity.Property(e => e.IdGvgd).HasColumnName("idGVGD");
            entity.Property(e => e.IdMh).HasColumnName("idMH");
            entity.Property(e => e.IdNh).HasColumnName("idNH");
            entity.Property(e => e.IdQt).HasColumnName("idQT");

            entity.HasOne(d => d.IdClassNavigation).WithMany(p => p.Pcgvgds)
                .HasForeignKey(d => d.IdClass)
                .HasConstraintName("FK_PCGVGD_Class");

            entity.HasOne(d => d.IdGvgdNavigation).WithMany(p => p.Pcgvgds)
                .HasForeignKey(d => d.IdGvgd)
                .HasConstraintName("FK_PCGVGD_GVGD");

            entity.HasOne(d => d.IdMhNavigation).WithMany(p => p.Pcgvgds)
                .HasForeignKey(d => d.IdMh)
                .HasConstraintName("FK_PCGVGD_Subject");

            entity.HasOne(d => d.IdNhNavigation).WithMany(p => p.Pcgvgds)
                .HasForeignKey(d => d.IdNh)
                .HasConstraintName("FK_PCGVGD_NH");

            entity.HasOne(d => d.IdQtNavigation).WithMany(p => p.Pcgvgds)
                .HasForeignKey(d => d.IdQt)
                .HasConstraintName("FK_PCGVGD_Progress");
        });

        modelBuilder.Entity<PointStudent>(entity =>
        {
            entity.ToTable("Point_Student");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Evaluate).HasMaxLength(50);
            entity.Property(e => e.IdMh).HasColumnName("idMH");
            entity.Property(e => e.IdQt).HasColumnName("idQT");
            entity.Property(e => e.IdSv).HasColumnName("idSV");

            entity.HasOne(d => d.IdMhNavigation).WithMany(p => p.PointStudents)
                .HasForeignKey(d => d.IdMh)
                .HasConstraintName("FK_Point_Student_Subject");

            entity.HasOne(d => d.IdQtNavigation).WithMany(p => p.PointStudents)
                .HasForeignKey(d => d.IdQt)
                .HasConstraintName("FK_Point_Student_Progress");

            entity.HasOne(d => d.IdSvNavigation).WithMany(p => p.PointStudents)
                .HasForeignKey(d => d.IdSv)
                .HasConstraintName("FK_Point_Student_Hocsinh");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.ToTable("Progress");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameProgress)
                .HasMaxLength(100)
                .HasColumnName("Name_progress");
            entity.Property(e => e.TimeCre).HasColumnName("time_cre");
            entity.Property(e => e.TimeUp).HasColumnName("time_up");
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

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaMh)
                .HasMaxLength(20)
                .HasColumnName("MaMH");
            entity.Property(e => e.NameSub)
                .HasMaxLength(200)
                .HasColumnName("Name_sub");
            entity.Property(e => e.TimeCre).HasColumnName("time_cre");
            entity.Property(e => e.TimeUp).HasColumnName("time_up");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
