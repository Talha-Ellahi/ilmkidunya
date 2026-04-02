using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.BackupModel3;

public partial class Dbikd4Context : DbContext
{
    public Dbikd4Context()
    {
    }

    public Dbikd4Context(DbContextOptions<Dbikd4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCm> TblCms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbikd4");

        modelBuilder.Entity<TblCm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblCms__3214EC0755A1A8B0");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeys).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(255);
            entity.Property(e => e.PageName).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
