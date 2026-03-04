using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.DictionaryModels;

public partial class DictionaryContext : DbContext
{
    public DictionaryContext()
    {
    }

    public DictionaryContext(DbContextOptions<DictionaryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Meaning> Meanings { get; set; }

    public virtual DbSet<TblCm> TblCms { get; set; }

    public virtual DbSet<Word> Words { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=173.214.175.66;Database=dictionaryIKD;User Id=userdictionaryIKD;Password=9AR1Kosqe?uny6%b;TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdictionaryIKD");

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B938558B197BA5B2");

            entity.HasIndex(e => e.Code, "UQ__Language__A25C5AA7F00D5313").IsUnique();

            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Meaning>(entity =>
        {
            entity.HasKey(e => e.MeaningId).HasName("PK__Meaning__3F75CBA7E58E84FA");

            entity.ToTable("Meaning");
        });

        modelBuilder.Entity<TblCm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblCm__3214EC079D5FBEAE");

            entity.ToTable("TblCm");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeys).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(255);
            entity.Property(e => e.PageName).HasMaxLength(255);
            entity.Property(e => e.ShortHeading).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__Words__2C20F046946E996E");

            entity.HasIndex(e => new { e.LanguageId, e.Word1 }, "UQ_Words").IsUnique();

            entity.HasIndex(e => new { e.BucketKey, e.Word1 }, "idx_bucket_word");

            entity.HasIndex(e => new { e.LanguageId, e.Word1 }, "idx_lang_word");

            entity.Property(e => e.WordId).HasColumnName("WordID");
            entity.Property(e => e.BucketKey)
                .HasMaxLength(50)
                .UseCollation("Arabic_100_CI_AS_SC");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.PartOfSpeech).HasMaxLength(50);
            entity.Property(e => e.Word1)
                .HasMaxLength(255)
                .HasColumnName("Word");

            entity.HasOne(d => d.Language).WithMany(p => p.Words)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Words_Languages");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
