using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.DBCollege;

public partial class DbCollegeContext : DbContext
{
    public DbCollegeContext()
    {
    }

    public DbCollegeContext(DbContextOptions<DbCollegeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseCategoryJoin> CourseCategoryJoins { get; set; }

    public virtual DbSet<InstituteType> InstituteTypes { get; set; }

    public virtual DbSet<SectionContentImport> SectionContentImports { get; set; }

    public virtual DbSet<SectionTypeImport> SectionTypeImports { get; set; }

    public virtual DbSet<TblAdmission> TblAdmissions { get; set; }

    public virtual DbSet<TblAdmissionCourse> TblAdmissionCourses { get; set; }

    public virtual DbSet<TblCollege> TblColleges { get; set; }

    public virtual DbSet<TblCollegereview> TblCollegereviews { get; set; }

    public virtual DbSet<TblCourseinquiry> TblCourseinquiries { get; set; }

    public virtual DbSet<TblDefCity> TblDefCities { get; set; }

    public virtual DbSet<TblGuidesDefination> TblGuidesDefinations { get; set; }

    public virtual DbSet<TblMeritList> TblMeritLists { get; set; }

    public virtual DbSet<TblMeritListType> TblMeritListTypes { get; set; }

    public virtual DbSet<TblXcourseLevel> TblXcourseLevels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=66.23.236.70;Initial Catalog=dbcolleges;User Id=userdbcolleges;Password=Hw0ST9t^nbou!ly6;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbcolleges");

        modelBuilder.Entity<Board>(entity =>
        {
            entity.ToTable("Boards", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
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

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Coursetags).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Duration).HasMaxLength(255);
            entity.Property(e => e.EducationLevelId).HasColumnName("EducationLevelID");
            entity.Property(e => e.Fee)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FeeDescription).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SemesterPlanFile).HasMaxLength(255);
            entity.Property(e => e.Specializations).HasMaxLength(255);
            entity.Property(e => e.SpecializationsFile).HasMaxLength(255);
            entity.Property(e => e.Syllabusfile).HasMaxLength(255);
            entity.Property(e => e.TotalFee)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.ToTable("CourseCategory", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<CourseCategoryJoin>(entity =>
        {
            entity.ToTable("CourseCategoryJoin", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<InstituteType>(entity =>
        {
            entity.ToTable("InstituteType", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.MetaDesc).HasMaxLength(600);
            entity.Property(e => e.MetaKeyword).HasMaxLength(600);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(600);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SectionContentImport>(entity =>
        {
            entity.ToTable("SectionContentImport", "dbikduser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
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
            entity.ToTable("SectionTypeImport", "dbikduser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
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

        modelBuilder.Entity<TblAdmission>(entity =>
        {
            entity.ToTable("tblAdmissions", "dbikduser");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<TblAdmissionCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblAdmissionCourses", "dbikduser");
        });

        modelBuilder.Entity<TblCollege>(entity =>
        {
            entity.ToTable("TblCollege", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContactNumber).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateSheetUrl).HasColumnName("DateSheetURL");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fbranking).HasColumnName("FBRanking");
            entity.Property(e => e.FemaleStudents).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hecranking).HasColumnName("HECRanking");
            entity.Property(e => e.Hecreview).HasColumnName("HECReview");
            entity.Property(e => e.IsGovt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Latitude)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Logo).HasMaxLength(255);
            entity.Property(e => e.Longitude)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaleStudents).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ProspectusUrl).HasColumnName("ProspectusURL");
            entity.Property(e => e.Qsranking).HasColumnName("QSRanking");
            entity.Property(e => e.ResultUrl).HasColumnName("ResultURL");
            entity.Property(e => e.ShortName).HasMaxLength(100);
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.TitleImage).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Views).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Website).HasMaxLength(255);
            entity.Property(e => e.Zoomlevel)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblCollegereview>(entity =>
        {
            entity.ToTable("tbl_Collegereviews", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.Date).HasColumnType("smalldatetime");
            entity.Property(e => e.InstId).HasColumnName("inst_id");
        });

        modelBuilder.Entity<TblCourseinquiry>(entity =>
        {
            entity.ToTable("tblCourseinquiry", "dbikduser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CollegeId).HasColumnName("CollegeID");
            entity.Property(e => e.CurrentDegree).HasMaxLength(200);
            entity.Property(e => e.DivisionId).HasColumnName("DivisionID");
            entity.Property(e => e.GraduationYear).HasColumnName("Graduation Year");
            entity.Property(e => e.GuideId).HasColumnName("GuideID");
            entity.Property(e => e.Inquirydate).HasColumnName("inquirydate");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<TblDefCity>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("tblDefCity", "dbo");

            entity.Property(e => e.CityId)
                .ValueGeneratedNever()
                .HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(80)
                .HasColumnName("city_name");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTitle).HasMaxLength(100);
            entity.Property(e => e.PostalCode).IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblGuidesDefination>(entity =>
        {
            entity.ToTable("TblGuidesDefination", "dbikduser");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Abrevation).HasMaxLength(100);
            entity.Property(e => e.CategoryIds).HasMaxLength(500);
            entity.Property(e => e.CourseUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GuideAdmissionPageUrl).HasMaxLength(500);
            entity.Property(e => e.GuideFeeStructurePageUrl).HasMaxLength(500);
            entity.Property(e => e.GuideMainUrl).HasMaxLength(500);
            entity.Property(e => e.GuideMeritListPageUrl).HasMaxLength(500);
            entity.Property(e => e.GuideName).HasMaxLength(255);
            entity.Property(e => e.GuideUniversityListPageUrl).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblMeritList>(entity =>
        {
            entity.ToTable("tblMeritList", "dbikduser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AddedDate).HasColumnType("datetime");
            entity.Property(e => e.CollegeId).HasColumnName("CollegeID");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MeritListTypeId).HasColumnName("MeritListTypeID");
            entity.Property(e => e.MeritValue)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblMeritListType>(entity =>
        {
            entity.ToTable("TblMeritListType", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.MeritListTypeName).IsUnicode(false);
        });

        modelBuilder.Entity<TblXcourseLevel>(entity =>
        {
            entity.ToTable("tblXCourseLevels", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
