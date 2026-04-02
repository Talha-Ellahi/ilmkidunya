using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.JobModels;

public partial class JobsDbContext : DbContext
{
    public JobsDbContext()
    {
    }

    public JobsDbContext(DbContextOptions<JobsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<JobAd> JobAds { get; set; }

    public virtual DbSet<JobType> JobTypes { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<SubJobCategory> SubJobCategories { get; set; }

    public virtual DbSet<TblDefCity> TblDefCities { get; set; }

    public virtual DbSet<TblJobScale> TblJobScales { get; set; }

    public virtual DbSet<Tbljobadslatest> Tbljobadslatests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=69.10.38.62;Database=dbjobsikd;User Id=userdbjobsikd;Password=L1cg1T@Dlag%woz2;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbjobsikd");

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.Dated).HasColumnType("smalldatetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Heading).HasMaxLength(200);
            entity.Property(e => e.LiveUrl).IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTitle).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.ShortForm).HasMaxLength(300);
            entity.Property(e => e.Url).HasMaxLength(600);
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobAd>(entity =>
        {
            entity.ToTable("JobAds", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AbroadCityName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.AgeLimit).HasMaxLength(200);
            entity.Property(e => e.ApplyOnlineUrl).HasMaxLength(1000);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Detail).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Experience).HasMaxLength(100);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsCities)
                .HasMaxLength(600)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsCitiesIds)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("JobAdsCitiesIDs");
            entity.Property(e => e.JobAdsNewsPapers)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsNewsPapersIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("JobAdsNewsPapersIDs");
            entity.Property(e => e.JobAdsTypes)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsTypesIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("JobAdsTypesIDs");
            entity.Property(e => e.JobScaleId).HasColumnName("JobScaleID");
            entity.Property(e => e.JobSkills)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.JobViews).HasDefaultValue(1);
            entity.Property(e => e.JobsFromMustakbil).HasDefaultValue(false);
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.MaxSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MemberId)
                .HasDefaultValue(0)
                .HasColumnName("MemberID");
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(400);
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MinSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Otstest)
                .IsUnicode(false)
                .HasColumnName("OTSTest");
            entity.Property(e => e.ParentCompanyId).HasColumnName("ParentCompanyID");
            entity.Property(e => e.ParentJobIdNew).HasColumnName("ParentJobID_New");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.QualificationId).HasColumnName("QualificationID");
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.TestSpid).HasColumnName("TestSPID");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.VideoEmbedUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<JobType>(entity =>
        {
            entity.ToTable("JobType", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(80);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.ToTable("Qualification", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
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

        modelBuilder.Entity<SubJobCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubJobCa__3214EC2719BDC35D");

            entity.ToTable("SubJobCategories", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Detail).IsUnicode(false);
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
            entity.Property(e => e.MetaDesc).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(80);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblDefCity>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("tblDefCity", "dbo");

            entity.Property(e => e.CityId).HasColumnName("city_id");
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

        modelBuilder.Entity<TblJobScale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblJobSc__3214EC275A857525");

            entity.ToTable("tblJobScale", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Tbljobadslatest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JobAdsLatest");

            entity.ToTable("Tbljobadslatest", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AbroadCityName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.AgeLimit).HasMaxLength(200);
            entity.Property(e => e.ApplyOnlineUrl).HasMaxLength(1000);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Detail).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Experience).HasMaxLength(100);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsCities)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsCitiesIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("JobAdsCitiesIDs");
            entity.Property(e => e.JobAdsNewsPapers)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsNewsPapersIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("JobAdsNewsPapersIDs");
            entity.Property(e => e.JobAdsTypes)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.JobAdsTypesIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("JobAdsTypesIDs");
            entity.Property(e => e.JobScaleId).HasColumnName("JobScaleID");
            entity.Property(e => e.JobSkills)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.MaxSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(400);
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MinSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Otstest)
                .IsUnicode(false)
                .HasColumnName("OTSTest");
            entity.Property(e => e.ParentCompanyId).HasColumnName("ParentCompanyID");
            entity.Property(e => e.ParentJobIdNew).HasColumnName("ParentJobID_New");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.QualificationId).HasColumnName("QualificationID");
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.TestSpid).HasColumnName("TestSPID");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.VideoEmbedUrl).HasMaxLength(1000);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
