using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.BackupModel2;

public partial class Dbikd2Context : DbContext
{
    public Dbikd2Context()
    {
    }

    public Dbikd2Context(DbContextOptions<Dbikd2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCm> TblCms { get; set; }

    public virtual DbSet<TblCms3> TblCms3s { get; set; }

    public virtual DbSet<VwTop22Cm> VwTop22Cms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=173.214.175.66;Database=dbikd2_26;User Id=userdbikd2_26;Password=?YiJg0fRpqusg6%4;TrustServerCertificate=True;MultipleActiveResultSets=true;Connection Timeout=30;Command Timeout=60;Pooling=true;Min Pool Size=5;Max Pool Size=100;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbikd2_26");

        modelBuilder.Entity<TblCm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblCms2__3214EC27886768C4");

            entity.ToTable("TblCms", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Heading)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MetaTitle).IsUnicode(false);
            entity.Property(e => e.PageName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<TblCms3>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblCms__3214EC0777A46E59");

            entity.ToTable("TblCms3", "dbo");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeys).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(255);
            entity.Property(e => e.PageName).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<VwTop22Cm>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Top22Cms", "dbo");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Heading)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MetaTitle).IsUnicode(false);
            entity.Property(e => e.PageName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("URL");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
