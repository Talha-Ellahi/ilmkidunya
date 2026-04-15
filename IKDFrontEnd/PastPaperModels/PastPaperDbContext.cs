using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.PastPaperModels;

public partial class PastPaperDbContext : DbContext
{
    public PastPaperDbContext()
    {
    }

    public PastPaperDbContext(DbContextOptions<PastPaperDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Boooard> Boooards { get; set; }

    public virtual DbSet<PastPaperPageDescription> PastPaperPageDescriptions { get; set; }

    public virtual DbSet<PastPaperQuestionType> PastPaperQuestionTypes { get; set; }

    public virtual DbSet<PastPaperType> PastPaperTypes { get; set; }

    public virtual DbSet<PastPaperTypeRelation> PastPaperTypeRelations { get; set; }

    public virtual DbSet<SectionContentImport> SectionContentImports { get; set; }

    public virtual DbSet<SectionTypeImport> SectionTypeImports { get; set; }

    public virtual DbSet<TblBoard> TblBoards { get; set; }

    public virtual DbSet<TblPastPaper> TblPastPapers { get; set; }

    public virtual DbSet<TblPpboardClass> TblPpboardClasses { get; set; }

    public virtual DbSet<TblPpboardClassSubject> TblPpboardClassSubjects { get; set; }

    public virtual DbSet<TblPpclass> TblPpclasses { get; set; }

    public virtual DbSet<TblPpqualification> TblPpqualifications { get; set; }

    public virtual DbSet<TblPpsubject> TblPpsubjects { get; set; }

    public virtual DbSet<TblPptype> TblPptypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=67.217.56.210;Database=dbpapers;User Id=dbuserpapers;Password=^Ei4uaJd1H6$buwx;TrustServerCertificate=True;MultipleActiveResultSets=true;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbuserpapers");

        modelBuilder.Entity<Board>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.LevelIds)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LevelIDs");
            entity.Property(e => e.LiveUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Boooard>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.LevelIds)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LevelIDs");
            entity.Property(e => e.LiveUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PastPaperPageDescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PastPape__3214EC07E5DD83E0");

            entity.ToTable("PastPaperPageDescription");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<PastPaperQuestionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PastPape__3214EC27775B7170");

            entity.ToTable("PastPaperQuestionType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<PastPaperType>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<PastPaperTypeRelation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PastPape__3214EC272C472CF3");

            entity.ToTable("PastPaperTypeRelation");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.PastPaperId).HasColumnName("PastPaperID");
            entity.Property(e => e.PastPaperTypeId).HasColumnName("PastPaperTypeID");
        });

        modelBuilder.Entity<SectionContentImport>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SectionContentImport");

            entity.Property(e => e.AppHeading).IsUnicode(false);
            entity.Property(e => e.CollegeTypeId).HasMaxLength(250);
            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DetailShort).HasMaxLength(4000);
            entity.Property(e => e.FbHeader).HasMaxLength(2000);
            entity.Property(e => e.GenderId).HasMaxLength(250);
            entity.Property(e => e.GoogleAdFooter)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.HeaderText).IsUnicode(false);
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IconImage).IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Keywords).HasMaxLength(500);
            entity.Property(e => e.MainHeading)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(600);
            entity.Property(e => e.MetaKeyword).HasMaxLength(600);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(600);
            entity.Property(e => e.PageLink)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SectionTypeImport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SectionType");

            entity.ToTable("SectionTypeImport");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteTypeId).HasColumnName("InstituteTypeID");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblBoard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblBoard__3214EC0733AAB81B");

            entity.HasIndex(e => e.Slug, "UQ__TblBoard__BC7B5FB6509C68E8").IsUnique();

            entity.Property(e => e.BoardName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<TblPastPaper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPastP__3214EC2726685097");

            entity.ToTable("tblPastPapers");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Pdf).HasColumnName("PDF");
            entity.Property(e => e.Pnname)
                .HasMaxLength(255)
                .HasColumnName("PNName");
            entity.Property(e => e.PpclassId).HasColumnName("PPClassID");
            entity.Property(e => e.PpsubjectId).HasColumnName("PPSubjectID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Board).WithMany(p => p.TblPastPapers)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPastPapers_Boards");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPastPapers)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPastPa__PPCla__6754599E");

            entity.HasOne(d => d.Ppsubject).WithMany(p => p.TblPastPapers)
                .HasForeignKey(d => d.PpsubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPastPa__PPSub__68487DD7");
        });

        modelBuilder.Entity<TblPpboardClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPBoa__3214EC2797A48474");

            entity.ToTable("tblPPBoardClasses");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.PpclassId).HasColumnName("PPClassID");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPpboardClasses)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPPBoardClasses_tblPPClass");
        });

        modelBuilder.Entity<TblPpboardClassSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPBoa__3214EC27EE67A061");

            entity.ToTable("tblPPBoardClassSubjects");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.PpclassId).HasColumnName("PPClassID");
            entity.Property(e => e.PpsubjectId).HasColumnName("PPSubjectID");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPpboardClassSubjects)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__PPCla__75A278F5");

            entity.HasOne(d => d.Ppsubject).WithMany(p => p.TblPpboardClassSubjects)
                .HasForeignKey(d => d.PpsubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__PPSub__76969D2E");
        });

        modelBuilder.Entity<TblPpclass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPCla__3214EC27A8A9B957");

            entity.ToTable("tblPPClass");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PpclassName)
                .HasMaxLength(255)
                .HasColumnName("PPClassName");
            entity.Property(e => e.PpclassUrl).HasColumnName("PPClassURL");
            entity.Property(e => e.PpqualificationId).HasColumnName("PPQualificationID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Ppqualification).WithMany(p => p.TblPpclasses)
                .HasForeignKey(d => d.PpqualificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPClas__PPQua__656C112C");
        });

        modelBuilder.Entity<TblPpqualification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPQua__3214EC2771FCAC6C");

            entity.ToTable("tblPPQualification");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PpqualificationName).HasColumnName("PPQualificationName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblPpsubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPSub__3214EC27B2AE1FD3");

            entity.ToTable("tblPPSubject");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PpsubjectName).HasColumnName("PPSubjectName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblPptype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPTyp__3214EC2764D15F87");

            entity.ToTable("tblPPTypes");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PptypeName)
                .HasMaxLength(255)
                .HasColumnName("PPTypeName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
