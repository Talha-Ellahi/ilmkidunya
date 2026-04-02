using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class PastPaperDbContext : DbContext
{
    public PastPaperDbContext()
    {
    }

    public PastPaperDbContext(DbContextOptions<PastPaperDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AbroadCity> AbroadCities { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<BannerType> BannerTypes { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<BoardType> BoardTypes { get; set; }

    public virtual DbSet<Boooard> Boooards { get; set; }

    public virtual DbSet<CategoryTest> CategoryTests { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<City1> Citys { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassesSubject> ClassesSubjects { get; set; }

    public virtual DbSet<Classmigration> Classmigrations { get; set; }

    public virtual DbSet<Collegeformdatum> Collegeformdata { get; set; }

    public virtual DbSet<ComapnyAddress> ComapnyAddresses { get; set; }

    public virtual DbSet<CompanyFollow> CompanyFollows { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseCategoryJoin> CourseCategoryJoins { get; set; }

    public virtual DbSet<DateSheetResultType> DateSheetResultTypes { get; set; }

    public virtual DbSet<DefinitionSeo> DefinitionSeos { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<InstituteType> InstituteTypes { get; set; }

    public virtual DbSet<JobAdsCity> JobAdsCities { get; set; }

    public virtual DbSet<JobAdsClone> JobAdsClones { get; set; }

    public virtual DbSet<JobAdsPapular> JobAdsPapulars { get; set; }

    public virtual DbSet<JobCustomFile> JobCustomFiles { get; set; }

    public virtual DbSet<JobHotSpotRelation> JobHotSpotRelations { get; set; }

    public virtual DbSet<JobImagesHotSpot> JobImagesHotSpots { get; set; }

    public virtual DbSet<JobSearch> JobSearches { get; set; }

    public virtual DbSet<JobSubscriber> JobSubscribers { get; set; }

    public virtual DbSet<JobSubscriberChild> JobSubscriberChildren { get; set; }

    public virtual DbSet<JobUserAlert> JobUserAlerts { get; set; }

    public virtual DbSet<JobUserAlertChild> JobUserAlertChildren { get; set; }

    public virtual DbSet<JobVacancy> JobVacancies { get; set; }

    public virtual DbSet<JobsCustomCompanyCriterion> JobsCustomCompanyCriteria { get; set; }

    public virtual DbSet<JobsCustomCriteriaPage> JobsCustomCriteriaPages { get; set; }

    public virtual DbSet<JobsLastTwoDaysEmailAlert> JobsLastTwoDaysEmailAlerts { get; set; }

    public virtual DbSet<LongQuestionCriteriaDetail> LongQuestionCriteriaDetails { get; set; }

    public virtual DbSet<LongQuestionCriterion> LongQuestionCriteria { get; set; }

    public virtual DbSet<MovedCompany> MovedCompanies { get; set; }

    public virtual DbSet<MovedJobAd> MovedJobAds { get; set; }

    public virtual DbSet<MovedJobAdsNewTest> MovedJobAdsNewTests { get; set; }

    public virtual DbSet<MovedJobType> MovedJobTypes { get; set; }

    public virtual DbSet<MovedTblJobScale> MovedTblJobScales { get; set; }

    public virtual DbSet<MovedTbljobadslatest> MovedTbljobadslatests { get; set; }

    public virtual DbSet<NewsPaper> NewsPapers { get; set; }

    public virtual DbSet<NtsCompanyCustomPage> NtsCompanyCustomPages { get; set; }

    public virtual DbSet<OtsLearnMoreDatum> OtsLearnMoreData { get; set; }

    public virtual DbSet<PastPaperPageDescription> PastPaperPageDescriptions { get; set; }

    public virtual DbSet<PastPaperQuestionType> PastPaperQuestionTypes { get; set; }

    public virtual DbSet<PastPaperType> PastPaperTypes { get; set; }

    public virtual DbSet<PastPaperTypeRelation> PastPaperTypeRelations { get; set; }

    public virtual DbSet<PrivateMessage> PrivateMessages { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<SectionContentImport> SectionContentImports { get; set; }

    public virtual DbSet<SectionTypeImport> SectionTypeImports { get; set; }

    public virtual DbSet<ShortQuestionCriteriaDetail> ShortQuestionCriteriaDetails { get; set; }

    public virtual DbSet<ShortQuestionCriterion> ShortQuestionCriteria { get; set; }

    public virtual DbSet<ShortQuestionPreparation> ShortQuestionPreparations { get; set; }

    public virtual DbSet<ShortQuestionPreparationDetail> ShortQuestionPreparationDetails { get; set; }

    public virtual DbSet<ShortQuestionProblemsNotification> ShortQuestionProblemsNotifications { get; set; }

    public virtual DbSet<ShortQuestionsAnswerSuggestion> ShortQuestionsAnswerSuggestions { get; set; }

    public virtual DbSet<SubJobCategory> SubJobCategories { get; set; }

    public virtual DbSet<TblAdmission> TblAdmissions { get; set; }

    public virtual DbSet<TblAdmissionCourse> TblAdmissionCourses { get; set; }

    public virtual DbSet<TblAllGuidesCm> TblAllGuidesCms { get; set; }

    public virtual DbSet<TblAnswerChoice> TblAnswerChoices { get; set; }

    public virtual DbSet<TblAnswerChoicesTemp> TblAnswerChoicesTemps { get; set; }

    public virtual DbSet<TblArticle> TblArticles { get; set; }

    public virtual DbSet<TblArticleType> TblArticleTypes { get; set; }

    public virtual DbSet<TblBoard> TblBoards { get; set; }

    public virtual DbSet<TblCategoryOtsCustom> TblCategoryOtsCustoms { get; set; }

    public virtual DbSet<TblChapter> TblChapters { get; set; }

    public virtual DbSet<TblCityArea> TblCityAreas { get; set; }

    public virtual DbSet<TblClass> TblClasses { get; set; }

    public virtual DbSet<TblClassesChild> TblClassesChildren { get; set; }

    public virtual DbSet<TblCm> TblCms { get; set; }

    public virtual DbSet<TblCollege> TblColleges { get; set; }

    public virtual DbSet<TblCollegereview> TblCollegereviews { get; set; }

    public virtual DbSet<TblComments2> TblComments2s { get; set; }

    public virtual DbSet<TblCommentsChild> TblCommentsChildren { get; set; }

    public virtual DbSet<TblConsultantCountry> TblConsultantCountries { get; set; }

    public virtual DbSet<TblContactMessage> TblContactMessages { get; set; }

    public virtual DbSet<TblCourseinquiry> TblCourseinquiries { get; set; }

    public virtual DbSet<TblCustomFile> TblCustomFiles { get; set; }

    public virtual DbSet<TblDateSheetCriteriaChild> TblDateSheetCriteriaChildren { get; set; }

    public virtual DbSet<TblDateSheetcriterion> TblDateSheetcriteria { get; set; }

    public virtual DbSet<TblDefCity> TblDefCities { get; set; }

    public virtual DbSet<TblDefComment> TblDefComments { get; set; }

    public virtual DbSet<TblDefCommentLike> TblDefCommentLikes { get; set; }

    public virtual DbSet<TblDefCommentNew> TblDefCommentNews { get; set; }

    public virtual DbSet<TblDefConsultant> TblDefConsultants { get; set; }

    public virtual DbSet<TblDefCountry> TblDefCountries { get; set; }

    public virtual DbSet<TblDefMemberInfo2> TblDefMemberInfo2s { get; set; }

    public virtual DbSet<TblDefMemberInfoikd> TblDefMemberInfoikds { get; set; }

    public virtual DbSet<TblDefSubject> TblDefSubjects { get; set; }

    public virtual DbSet<TblEducationLevel> TblEducationLevels { get; set; }

    public virtual DbSet<TblFile> TblFiles { get; set; }

    public virtual DbSet<TblFlag> TblFlags { get; set; }

    public virtual DbSet<TblGuide> TblGuides { get; set; }

    public virtual DbSet<TblGuideDetail> TblGuideDetails { get; set; }

    public virtual DbSet<TblGuidesDefination> TblGuidesDefinations { get; set; }

    public virtual DbSet<TblHostel> TblHostels { get; set; }

    public virtual DbSet<TblHostelFacility> TblHostelFacilities { get; set; }

    public virtual DbSet<TblHostelFeature> TblHostelFeatures { get; set; }

    public virtual DbSet<TblHostelImage> TblHostelImages { get; set; }

    public virtual DbSet<TblHostelRoomType> TblHostelRoomTypes { get; set; }

    public virtual DbSet<TblInstitute> TblInstitutes { get; set; }

    public virtual DbSet<TblLecture> TblLectures { get; set; }

    public virtual DbSet<TblLectureChapter> TblLectureChapters { get; set; }

    public virtual DbSet<TblLectureClass> TblLectureClasses { get; set; }

    public virtual DbSet<TblLectureSubject> TblLectureSubjects { get; set; }

    public virtual DbSet<TblLectureTeacher> TblLectureTeachers { get; set; }

    public virtual DbSet<TblLectureTopic> TblLectureTopics { get; set; }

    public virtual DbSet<TblLongQuestion> TblLongQuestions { get; set; }

    public virtual DbSet<TblLongQuestionAnswerChoice> TblLongQuestionAnswerChoices { get; set; }

    public virtual DbSet<TblMainNews> TblMainNews { get; set; }

    public virtual DbSet<TblMemberTestHistory2> TblMemberTestHistory2s { get; set; }

    public virtual DbSet<TblMeritList> TblMeritLists { get; set; }

    public virtual DbSet<TblMeritListType> TblMeritListTypes { get; set; }

    public virtual DbSet<TblNewsCategory> TblNewsCategories { get; set; }

    public virtual DbSet<TblNewsCollege> TblNewsColleges { get; set; }

    public virtual DbSet<TblNewsComment> TblNewsComments { get; set; }

    public virtual DbSet<TblNewsLetter> TblNewsLetters { get; set; }

    public virtual DbSet<TblNewsLink> TblNewsLinks { get; set; }

    public virtual DbSet<TblNewsMultiCategory> TblNewsMultiCategories { get; set; }

    public virtual DbSet<TblOtsChapter> TblOtsChapters { get; set; }

    public virtual DbSet<TblOtsTestCriteriaDetail> TblOtsTestCriteriaDetails { get; set; }

    public virtual DbSet<TblOtsTestCriterion> TblOtsTestCriteria { get; set; }

    public virtual DbSet<TblOtsTestMcq> TblOtsTestMcqs { get; set; }

    public virtual DbSet<TblOtsTopic> TblOtsTopics { get; set; }

    public virtual DbSet<TblOtsclass> TblOtsclasses { get; set; }

    public virtual DbSet<TblOtsmcqchild> TblOtsmcqchildren { get; set; }

    public virtual DbSet<TblOtsquiz> TblOtsquizzes { get; set; }

    public virtual DbSet<TblOtsquizCategory> TblOtsquizCategories { get; set; }

    public virtual DbSet<TblOtsquizChild> TblOtsquizChildren { get; set; }

    public virtual DbSet<TblOtssubject> TblOtssubjects { get; set; }

    public virtual DbSet<TblPageFeedback> TblPageFeedbacks { get; set; }

    public virtual DbSet<TblPastPaper> TblPastPapers { get; set; }

    public virtual DbSet<TblPastPaperType> TblPastPaperTypes { get; set; }

    public virtual DbSet<TblPlaceOfStudy> TblPlaceOfStudies { get; set; }

    public virtual DbSet<TblPpboardClass> TblPpboardClasses { get; set; }

    public virtual DbSet<TblPpboardClassSubject> TblPpboardClassSubjects { get; set; }

    public virtual DbSet<TblPpclass> TblPpclasses { get; set; }

    public virtual DbSet<TblPpqualification> TblPpqualifications { get; set; }

    public virtual DbSet<TblPpsubject> TblPpsubjects { get; set; }

    public virtual DbSet<TblPptype> TblPptypes { get; set; }

    public virtual DbSet<TblResult> TblResults { get; set; }

    public virtual DbSet<TblResultCategory> TblResultCategories { get; set; }

    public virtual DbSet<TblResultClass> TblResultClasses { get; set; }

    public virtual DbSet<TblResultCriteriaChild> TblResultCriteriaChildren { get; set; }

    public virtual DbSet<TblResultCriterion> TblResultCriteria { get; set; }

    public virtual DbSet<TblResultUpdate> TblResultUpdates { get; set; }

    public virtual DbSet<TblSch> TblSches { get; set; }

    public virtual DbSet<TblSchFieldofStudy> TblSchFieldofStudies { get; set; }

    public virtual DbSet<TblSchFieldsofStudyChild> TblSchFieldsofStudyChildren { get; set; }

    public virtual DbSet<TblSchStudyLevel> TblSchStudyLevels { get; set; }

    public virtual DbSet<TblSchStudyLevelChild> TblSchStudyLevelChildren { get; set; }

    public virtual DbSet<TblSchTop> TblSchTops { get; set; }

    public virtual DbSet<TblSchTopLinking> TblSchTopLinkings { get; set; }

    public virtual DbSet<TblSchWithCategory> TblSchWithCategories { get; set; }

    public virtual DbSet<TblSection> TblSections { get; set; }

    public virtual DbSet<TblShortQuestion> TblShortQuestions { get; set; }

    public virtual DbSet<TblShortQuestionAnswerChoice> TblShortQuestionAnswerChoices { get; set; }

    public virtual DbSet<TblSlide> TblSlides { get; set; }

    public virtual DbSet<TblSlide1> TblSlides1 { get; set; }

    public virtual DbSet<TblSlider> TblSliders { get; set; }

    public virtual DbSet<TblSliderCategory> TblSliderCategories { get; set; }

    public virtual DbSet<TblStudyAbroadGuide> TblStudyAbroadGuides { get; set; }

    public virtual DbSet<TblStudyGuide> TblStudyGuides { get; set; }

    public virtual DbSet<TblSubject> TblSubjects { get; set; }

    public virtual DbSet<TblTeacher> TblTeachers { get; set; }

    public virtual DbSet<TblTest> TblTests { get; set; }

    public virtual DbSet<TblTltutor> TblTltutors { get; set; }

    public virtual DbSet<TblTutorFavourite> TblTutorFavourites { get; set; }

    public virtual DbSet<TblTutorLevel> TblTutorLevels { get; set; }

    public virtual DbSet<TblTutorMessage> TblTutorMessages { get; set; }

    public virtual DbSet<TblTutorSubject> TblTutorSubjects { get; set; }

    public virtual DbSet<TblUrlcontentMigrate> TblUrlcontentMigrates { get; set; }

    public virtual DbSet<TblWhatsAppGroup> TblWhatsAppGroups { get; set; }

    public virtual DbSet<TblXcourseLevel> TblXcourseLevels { get; set; }

    public virtual DbSet<Tblarticlestypemetadatum> Tblarticlestypemetadata { get; set; }

    public virtual DbSet<Tbldefmembertype2> Tbldefmembertype2s { get; set; }

    public virtual DbSet<Tblpagewisecontent> Tblpagewisecontents { get; set; }

    public virtual DbSet<Tblxinsttype> Tblxinsttypes { get; set; }

    public virtual DbSet<TestServiceProvider> TestServiceProviders { get; set; }

    public virtual DbSet<TestServiceProviderRelationWithJobAd> TestServiceProviderRelationWithJobAds { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<TutorContactForm> TutorContactForms { get; set; }

    public virtual DbSet<TutorDocument> TutorDocuments { get; set; }

    public virtual DbSet<TutorInquiry> TutorInquiries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WebStorySlider> WebStorySliders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbedupastpaper");

        modelBuilder.Entity<AbroadCity>(entity =>
        {
            entity.ToTable("AbroadCity", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MetaKeyword)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LoginID");
            entity.Property(e => e.MacAddress)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pictures).HasMaxLength(255);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.ToTable("AspNetRoles", "dbikduser");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.ToTable("AspNetRoleClaims", "dbikduser");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "dbikduser");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles", "dbikduser");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.ToTable("AspNetUserClaims", "dbikduser");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.ToTable("AspNetUserLogins", "dbikduser");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.ToTable("AspNetUserTokens", "dbikduser");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC0702436F20");

            entity.ToTable("Banners", "dbo");

            entity.Property(e => e.Deadline).HasColumnType("datetime");
        });

        modelBuilder.Entity<BannerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BannerTy__3214EC078F281A27");

            entity.ToTable("BannerTypes", "dbo");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.ToTable("Boards", "dbo");

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

        modelBuilder.Entity<BoardType>(entity =>
        {
            entity.ToTable("BoardType", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Boooard>(entity =>
        {
            entity.ToTable("Boooards", "dbo");

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

        modelBuilder.Entity<CategoryTest>(entity =>
        {
            entity.ToTable("CategoryTests", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.TestId).HasColumnName("TestID");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatMess__3214EC07800C1917");

            entity.ToTable("ChatMessages", "dbikduser");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__tblCity__F2D21B767960A40D");

            entity.ToTable("City", "dbikduser");

            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("cityName");
            entity.Property(e => e.IsFavourite)
                .HasDefaultValue(false)
                .HasColumnName("isFavourite");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("longitude");
        });

        modelBuilder.Entity<City1>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B76C220EC5A");

            entity.ToTable("Citys", "dbikduser");

            entity.Property(e => e.CityName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsFavourite).HasColumnName("isFavourite");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Classes", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTitle).HasMaxLength(80);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.QualificationId).HasColumnName("QualificationID");
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ClassesSubject>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ClassesSubjects", "dbo");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
        });

        modelBuilder.Entity<Classmigration>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK_tblClasses");

            entity.ToTable("classmigration", "dbikduser");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("class_name");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EducationLevelId).HasColumnName("education_level_id");
        });

        modelBuilder.Entity<Collegeformdatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_collegeformdata");

            entity.ToTable("Collegeformdata", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CollegeId).HasColumnName("CollegeID");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ComapnyAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ComapnyA__3214EC27443F6ADB");

            entity.ToTable("ComapnyAddress", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContactNumber).IsUnicode(false);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.EstablishedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanyFollow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CompanyF__3214EC27CA219E80");

            entity.ToTable("CompanyFollow", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Detail)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(400);
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC071637B475");

            entity.ToTable("Courses", "dbo");

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
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC07DCE05E01");

            entity.ToTable("CourseCategory", "dbo");

            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<CourseCategoryJoin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC073219B594");

            entity.ToTable("CourseCategoryJoin", "dbo");
        });

        modelBuilder.Entity<DateSheetResultType>(entity =>
        {
            entity.ToTable("DateSheetResultTypes", "dbikduser");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.IsDateSheet).HasColumnName("isDateSheet");
        });

        modelBuilder.Entity<DefinitionSeo>(entity =>
        {
            entity.ToTable("DefinitionSeo", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.DefinitionId).HasColumnName("DefinitionID");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Detailw3).IsUnicode(false);
            entity.Property(e => e.Heading)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImageName).HasMaxLength(200);
            entity.Property(e => e.RelventId).HasColumnName("RelventID");
            entity.Property(e => e.RelventName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FriendRe__3214EC074D00BBB7");

            entity.ToTable("FriendRequests", "dbikduser");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendRequestReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequestSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_FriendRequests_Sender");
        });

        modelBuilder.Entity<InstituteType>(entity =>
        {
            entity.ToTable("InstituteType", "dbo");

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

        modelBuilder.Entity<JobAdsCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("JobAdsCity", "dbo");
        });

        modelBuilder.Entity<JobAdsClone>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("JobAdsClone", "dbo");

            entity.Property(e => e.AbroadCityName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(500);
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
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
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
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.TestSpid).HasColumnName("TestSPID");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobAdsPapular>(entity =>
        {
            entity.ToTable("JobAdsPapular", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(300);
            entity.Property(e => e.MetaKeyword).HasMaxLength(300);
            entity.Property(e => e.MetaTitle).HasMaxLength(80);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SearchingUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobCustomFile>(entity =>
        {
            entity.ToTable("JobCustomFiles", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobId).HasColumnName("JobID");
        });

        modelBuilder.Entity<JobHotSpotRelation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobHotSp__3214EC27DAEAF9FF");

            entity.ToTable("JobHotSpotRelation", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HotSpotId).HasColumnName("HotSpotID");
        });

        modelBuilder.Entity<JobImagesHotSpot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobImage__3214EC27EFE338A3");

            entity.ToTable("JobImagesHotSpot", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AreaId).HasColumnName("areaId");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.PostedDate).HasColumnType("datetime");
            entity.Property(e => e.Width).HasColumnName("width");
            entity.Property(e => e.X).HasColumnName("x");
            entity.Property(e => e.X1).HasColumnName("x1");
            entity.Property(e => e.Y).HasColumnName("y");
            entity.Property(e => e.Y1).HasColumnName("y1");
            entity.Property(e => e.Z).HasColumnName("z");
        });

        modelBuilder.Entity<JobSearch>(entity =>
        {
            entity.ToTable("JobSearch", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.JobCategoryId).HasColumnName("JobCategoryID");
            entity.Property(e => e.NewsPaperId).HasColumnName("NewsPaperID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<JobSubscriber>(entity =>
        {
            entity.ToTable("JobSubscriber", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.JobTypeIds)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("JobTypeIDs");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
        });

        modelBuilder.Entity<JobSubscriberChild>(entity =>
        {
            entity.ToTable("JobSubscriberChild", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobSubsCriberId).HasColumnName("JobSubsCriberID");
            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
        });

        modelBuilder.Entity<JobUserAlert>(entity =>
        {
            entity.ToTable("JobUserAlerts", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MaxjobId).HasColumnName("MAXJobID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
        });

        modelBuilder.Entity<JobUserAlertChild>(entity =>
        {
            entity.ToTable("JobUserAlertChild", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.JobUserAlertId).HasColumnName("JobUserAlertID");
        });

        modelBuilder.Entity<JobVacancy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobVacan__3214EC0783ECC257");

            entity.ToTable("JobVacancy", "dbo");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Experience)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Qualification)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobsCustomCompanyCriterion>(entity =>
        {
            entity.ToTable("JobsCustomCompanyCriteria", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryIds).HasColumnName("CategoryIDs");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.IsGovt)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobsCustomCriteriaPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobsCust__3214EC274CCBF02E");

            entity.ToTable("JobsCustomCriteriaPages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Keywords).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<JobsLastTwoDaysEmailAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobsLast__3214EC27D045FCC4");

            entity.ToTable("JobsLastTwoDaysEmailAlert", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<LongQuestionCriteriaDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LongQues__3214EC27310E06DD");

            entity.ToTable("LongQuestionCriteriaDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChapterIds)
                .IsUnicode(false)
                .HasColumnName("ChapterIDs");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.OtscriteriaId).HasColumnName("OTSCriteriaID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Time).IsUnicode(false);
        });

        modelBuilder.Entity<LongQuestionCriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LongQues__3214EC27314F6911");

            entity.ToTable("LongQuestionCriteria", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.ImageName).IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Tabs).HasColumnName("TABS");
            entity.Property(e => e.TestName).IsUnicode(false);
            entity.Property(e => e.Url).IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.LongQuestionCriteria)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LongQuestionCriteria_Admin");
        });

        modelBuilder.Entity<MovedCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Company");

            entity.ToTable("moved_Company", "dbo");

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

        modelBuilder.Entity<MovedJobAd>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JobAds");

            entity.ToTable("moved_JobAds", "dbo");

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

        modelBuilder.Entity<MovedJobAdsNewTest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobAds_N__3214EC278677DA10");

            entity.ToTable("moved_JobAds_New_Test", "dbo");

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
            entity.Property(e => e.Profession).HasMaxLength(200);
            entity.Property(e => e.QualificationId).HasColumnName("QualificationID");
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.TestSpid).HasColumnName("TestSPID");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.VideoEmbedUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<MovedJobType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JobType");

            entity.ToTable("moved_JobType", "dbo");

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

        modelBuilder.Entity<MovedTblJobScale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblJobSc__3214EC27A780E4C8");

            entity.ToTable("moved_tblJobScale", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<MovedTbljobadslatest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JobAdsLatest");

            entity.ToTable("moved_Tbljobadslatest", "dbo");

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

        modelBuilder.Entity<NewsPaper>(entity =>
        {
            entity.ToTable("NewsPaper", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Heading)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.HeadingDateWise)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(600);
            entity.Property(e => e.MetaKeyword).HasMaxLength(600);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(600);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.NewsPapers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_NewsPaper_Admin");
        });

        modelBuilder.Entity<NtsCompanyCustomPage>(entity =>
        {
            entity.ToTable("ntsCompanyCustomPages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomUrl).HasMaxLength(500);
            entity.Property(e => e.IsFeatured).HasColumnName("isFeatured");
            entity.Property(e => e.NtscompanyId).HasColumnName("NTSCompanyID");
            entity.Property(e => e.NtsjobId).HasColumnName("NTSJobID");
            entity.Property(e => e.PageTypeId).HasColumnName("PageTypeID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<OtsLearnMoreDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OtsLearn__3214EC074BE9CCD2");

            entity.ToTable("OtsLearnMoreData", "dbikduser");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PastPaperPageDescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PastPape__3214EC07DCF42E19");

            entity.ToTable("PastPaperPageDescription", "dbikduser");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<PastPaperQuestionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PastPape__3214EC278289E9CA");

            entity.ToTable("PastPaperQuestionType", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<PastPaperType>(entity =>
        {
            entity.ToTable("PastPaperTypes", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<PastPaperTypeRelation>(entity =>
        {
            entity.ToTable("PastPaperTypeRelation", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PastPaperId).HasColumnName("PastPaperID");
            entity.Property(e => e.PastPaperTypeId).HasColumnName("PastPaperTypeID");
        });

        modelBuilder.Entity<PrivateMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PrivateM__3214EC07C9133762");

            entity.ToTable("PrivateMessages", "dbikduser");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.PrivateMessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrivateMessages_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.PrivateMessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_PrivateMessages_Sender");
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

        modelBuilder.Entity<SectionContentImport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SectionContents");

            entity.ToTable("SectionContentImport", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
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
            entity.HasKey(e => e.Id).HasName("PK_SectionType");

            entity.ToTable("SectionTypeImport", "dbikduser");

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

        modelBuilder.Entity<ShortQuestionCriteriaDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC2788186005");

            entity.ToTable("ShortQuestionCriteriaDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChapterIds)
                .IsUnicode(false)
                .HasColumnName("ChapterIDs");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.OtscriteriaId).HasColumnName("OTSCriteriaID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Time).IsUnicode(false);
        });

        modelBuilder.Entity<ShortQuestionCriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC271988BD19");

            entity.ToTable("ShortQuestionCriteria", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ImageName).IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Tabs).HasColumnName("TABS");
            entity.Property(e => e.TestName).IsUnicode(false);
            entity.Property(e => e.Url).IsUnicode(false);
        });

        modelBuilder.Entity<ShortQuestionPreparation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC276A04D08F");

            entity.ToTable("ShortQuestionPreparation", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.IsBrowser).HasColumnName("isBrowser");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.PreparationId).HasColumnName("PreparationID");
            entity.Property(e => e.Url).IsUnicode(false);
        });

        modelBuilder.Entity<ShortQuestionPreparationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC276403652E");

            entity.ToTable("ShortQuestionPreparationDetails", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.TestId).HasColumnName("TestID");
        });

        modelBuilder.Entity<ShortQuestionProblemsNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC2716A9861D");

            entity.ToTable("ShortQuestionProblemsNotifications", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChapterName)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Eamil).HasMaxLength(100);
            entity.Property(e => e.GivenPreparationId)
                .IsUnicode(false)
                .HasColumnName("GivenPreparationID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Problem).IsUnicode(false);
            entity.Property(e => e.TestName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShortQuestionsAnswerSuggestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortQue__3214EC27146F2774");

            entity.ToTable("ShortQuestionsAnswerSuggestions", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<SubJobCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubJobCa__3214EC27B29BF442");

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

        modelBuilder.Entity<TblAdmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblAdmis__3214EC0701BC1617");

            entity.ToTable("tblAdmissions", "dbikduser");

            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.TblAdmissions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblAdmissions_Admin");
        });

        modelBuilder.Entity<TblAdmissionCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblAdmis__3214EC070DFC2BB8");

            entity.ToTable("tblAdmissionCourses", "dbikduser");
        });

        modelBuilder.Entity<TblAllGuidesCm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblAllGu__3214EC07EC36FF27");

            entity.ToTable("TblAllGuidesCms", "dbikduser");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.HeaderIcon).HasMaxLength(255);
            entity.Property(e => e.HeaderName).HasMaxLength(255);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
        });

        modelBuilder.Entity<TblAnswerChoice>(entity =>
        {
            entity.HasKey(e => e.ChoiceId);

            entity.ToTable("tblAnswerChoices", "dbo");

            entity.Property(e => e.ChoiceId).HasColumnName("choice_id");
            entity.Property(e => e.ChoiceDescription).HasColumnName("choice_description");
            entity.Property(e => e.ChoiceImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("choice_image");
            entity.Property(e => e.IsTrueChoice).HasColumnName("is_true_choice");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
        });

        modelBuilder.Entity<TblAnswerChoicesTemp>(entity =>
        {
            entity.HasKey(e => e.ChoiceId);

            entity.ToTable("tblAnswerChoicesTemp", "dbo");

            entity.Property(e => e.ChoiceId).HasColumnName("choice_id");
            entity.Property(e => e.ChoiceDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("choice_description");
            entity.Property(e => e.ChoiceImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("choice_image");
            entity.Property(e => e.IsTrueChoice).HasColumnName("is_true_choice");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
        });

        modelBuilder.Entity<TblArticle>(entity =>
        {
            entity.HasKey(e => e.ArticleId);

            entity.ToTable("tblArticles", "dbo");

            entity.Property(e => e.ArticleId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("article_id");
            entity.Property(e => e.Approve)
                .HasDefaultValue(false)
                .HasColumnName("approve");
            entity.Property(e => e.ArticleDescription)
                .HasMaxLength(500)
                .HasColumnName("article_description");
            entity.Property(e => e.ArticleDetails).HasColumnName("article_details");
            entity.Property(e => e.ArticleKeywords)
                .HasMaxLength(500)
                .HasColumnName("article_keywords");
            entity.Property(e => e.ArticleTypeId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("article_type_id");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.Filename).HasColumnName("filename");
            entity.Property(e => e.MemberId)
                .HasDefaultValue(13148m)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(500)
                .HasColumnName("meta_title");
            entity.Property(e => e.NewsTags)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("news_tags");
            entity.Property(e => e.OtherFile)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("other_file");
            entity.Property(e => e.Picture1)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("picture_1");
            entity.Property(e => e.PictureThumbnail)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("picture_thumbnail");
            entity.Property(e => e.RewriteUrl)
                .HasMaxLength(300)
                .HasColumnName("rewrite_url");
            entity.Property(e => e.SenderEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("sender_email");
            entity.Property(e => e.SenderName)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("sender_name");
            entity.Property(e => e.Source)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("source");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Viewed)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("viewed");

            entity.HasOne(d => d.User).WithMany(p => p.TblArticles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblArticles_Admin");
        });

        modelBuilder.Entity<TblArticleType>(entity =>
        {
            entity.HasKey(e => e.ArticleTypeId);

            entity.ToTable("tblArticleTypes", "dbo");

            entity.Property(e => e.ArticleTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("article_type_id");
            entity.Property(e => e.ArticleTypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("article_type_name");
            entity.Property(e => e.Icon)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("icon");
            entity.Property(e => e.RewriteUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("rewrite_url");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<TblBoard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblBoard__3214EC07D3858E4E");

            entity.ToTable("TblBoards", "dbikduser");

            entity.HasIndex(e => e.Slug, "UQ__TblBoard__BC7B5FB6C239A5B2").IsUnique();

            entity.Property(e => e.BoardName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<TblCategoryOtsCustom>(entity =>
        {
            entity.HasKey(e => e.CustomCatId);

            entity.ToTable("tbl_category_OTS_custom", "dbo");

            entity.Property(e => e.CustomCatId).HasColumnName("custom_cat_id");
            entity.Property(e => e.CatDesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("cat_desc");
            entity.Property(e => e.CatName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("cat_name");
            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.DetailedImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("detailedImage");
            entity.Property(e => e.ImageIcon)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image_Icon");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<TblChapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId);

            entity.ToTable("tblChapter", "dbo");

            entity.Property(e => e.ChapterId).HasColumnName("chapter_id");
            entity.Property(e => e.ChapterContent).HasColumnName("chapter_content");
            entity.Property(e => e.ChapterName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("chapter_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MetaKey).HasColumnName("meta_Key");
            entity.Property(e => e.MetaTitle).HasColumnName("meta_title");
            entity.Property(e => e.SortOrder).HasColumnName("sortOrder");
            entity.Property(e => e.SubjectId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("subject_id");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<TblCityArea>(entity =>
        {
            entity.ToTable("tblCityArea", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityArea).HasMaxLength(200);
        });

        modelBuilder.Entity<TblClass>(entity =>
        {
            entity.HasKey(e => e.ClassId);

            entity.ToTable("tblClasses", "dbo");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("class_name");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EducationLevelId).HasColumnName("education_level_id");
        });

        modelBuilder.Entity<TblClassesChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblClass__3214EC07A76CFD43");

            entity.ToTable("tblClassesChild", "dbikduser");

            entity.Property(e => e.MainClassName).HasMaxLength(100);
        });

        modelBuilder.Entity<TblCm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblCMS__3214EC27FAA1EFA8");

            entity.ToTable("tblCMS", "dbikduser");

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

        modelBuilder.Entity<TblCollege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblColle__3214EC07A1866B88");

            entity.ToTable("TblCollege", "dbo");

            entity.Property(e => e.ContactNumber).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateSheetUrl).HasColumnName("DateSheetURL");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fbranking).HasColumnName("FBRanking");
            entity.Property(e => e.FemaleStudents).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hecranking).HasColumnName("HECRanking");
            entity.Property(e => e.Hecreview).HasColumnName("HECReview");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.IsGovt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsMeritListTop).HasDefaultValue(false);
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

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.Date).HasColumnType("smalldatetime");
            entity.Property(e => e.InstId).HasColumnName("inst_id");
        });

        modelBuilder.Entity<TblComments2>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_tblComments");

            entity.ToTable("tblComments2", "dbikduser");

            entity.Property(e => e.CommentId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("comment_id");
            entity.Property(e => e.Comments)
                .HasMaxLength(2000)
                .HasColumnName("comments");
            entity.Property(e => e.DatePosted)
                .HasColumnType("datetime")
                .HasColumnName("date_posted");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Ipaddress)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved");
            entity.Property(e => e.ItemId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("item_id");
            entity.Property(e => e.MemberId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PageNo).HasColumnName("page_no");
            entity.Property(e => e.PageUrl)
                .HasMaxLength(1000)
                .HasColumnName("page_url");
            entity.Property(e => e.SectionName)
                .HasMaxLength(200)
                .HasColumnName("section_name");
            entity.Property(e => e.Source).HasMaxLength(50);
        });

        modelBuilder.Entity<TblCommentsChild>(entity =>
        {
            entity.HasKey(e => e.ChildCmtId).HasName("PK_tblComments_Like_Dislike_against_CmtId");

            entity.ToTable("tblComments_Child", "dbikduser");

            entity.Property(e => e.ChildCmtId).HasColumnName("ChildCmt_Id");
            entity.Property(e => e.Abused).HasDefaultValue(0);
            entity.Property(e => e.CId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("C_Id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("ipaddress");
            entity.Property(e => e.IsAdminReply).HasColumnName("isAdminReply");
            entity.Property(e => e.MemberId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.Posteddate)
                .HasMaxLength(50)
                .HasColumnName("posteddate");
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<TblConsultantCountry>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TblConsultantCountries", "dbikduser");

            entity.Property(e => e.CompanyId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("company_id");
            entity.Property(e => e.CountryId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("country_id");
            entity.Property(e => e.MemberId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("member_id");
        });

        modelBuilder.Entity<TblContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblConta__3214EC07592A5665");

            entity.ToTable("TblContactMessage", "dbikduser");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Subject).HasMaxLength(250);
        });

        modelBuilder.Entity<TblCourseinquiry>(entity =>
        {
            entity.ToTable("tblCourseinquiry", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CollegeId).HasColumnName("CollegeID");
            entity.Property(e => e.CurrentDegree).HasMaxLength(200);
            entity.Property(e => e.DivisionId).HasColumnName("DivisionID");
            entity.Property(e => e.GraduationYear).HasColumnName("Graduation Year");
            entity.Property(e => e.GuideId).HasColumnName("GuideID");
            entity.Property(e => e.Inquirydate).HasColumnName("inquirydate");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<TblCustomFile>(entity =>
        {
            entity.ToTable("tblCustomFiles", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomPageId).HasColumnName("CustomPageID");
        });

        modelBuilder.Entity<TblDateSheetCriteriaChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DateShee__3214EC076B7AF1A6");

            entity.ToTable("TblDateSheetCriteriaChild", "dbikduser");

            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.ExamDate).HasMaxLength(100);
            entity.Property(e => e.ExpectedDate).HasMaxLength(100);
        });

        modelBuilder.Entity<TblDateSheetcriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DateShee__3214EC0729EED797");

            entity.ToTable("TblDateSheetcriteria", "dbikduser");

            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.DatesheetCode).HasMaxLength(50);
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

        modelBuilder.Entity<TblDefComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__TblDefCo__C3B4DFCA156863CA");

            entity.ToTable("TblDefComments", "dbikduser");

            entity.HasIndex(e => e.CreatedDate, "IX_CreatedDate").IsDescending();

            entity.HasIndex(e => e.PageUrl, "IX_PageUrl");

            entity.HasIndex(e => e.ParentCommentId, "IX_ParentCommentId");

            entity.HasIndex(e => e.PageUrl, "IX_TblDefComments_PageUrl");

            entity.HasIndex(e => e.PageUrlNoSlash, "IX_TblDefComments_PageUrlNoSlash").HasFilter("([IsActive]=(1) AND [IsApproved]=(1) AND [ParentCommentId] IS NULL)");

            entity.HasIndex(e => e.ParentCommentId, "IX_TblDefComments_ParentCommentId");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.PageTitle).HasMaxLength(500);
            entity.Property(e => e.PageUrl).HasMaxLength(500);
            entity.Property(e => e.PageUrlNoSlash)
                .HasMaxLength(4000)
                .HasComputedColumnSql("(replace([PageUrl],'/',''))", true);
            entity.Property(e => e.UserEmail).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.UserProfilePicture).HasMaxLength(500);
        });

        modelBuilder.Entity<TblDefCommentLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__TblDefCo__A2922C14DFE3F571");

            entity.ToTable("TblDefCommentLikes", "dbikduser");

            entity.HasIndex(e => e.CommentId, "IX_CommentId");

            entity.HasIndex(e => e.CommentId, "IX_TblDefCommentLikes_CommentId");

            entity.HasIndex(e => e.UserId, "IX_TblDefCommentLikes_UserId");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.HasIndex(e => new { e.CommentId, e.UserId }, "UC_CommentUser").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Comment).WithMany(p => p.TblDefCommentLikes)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_CommentLikes_Comments");
        });

        modelBuilder.Entity<TblDefCommentNew>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__TblDefCo__C3B4DFCA159178D6");

            entity.ToTable("TblDefComment_New", "dbikduser");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PageTitle).HasMaxLength(500);
            entity.Property(e => e.PageUrl).HasMaxLength(500);
            entity.Property(e => e.PageUrlNoSlash).HasMaxLength(500);
            entity.Property(e => e.UserEmail).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserName).HasMaxLength(200);
            entity.Property(e => e.UserProfilePicture).HasMaxLength(500);
        });

        modelBuilder.Entity<TblDefConsultant>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK_tblDefConsultants");

            entity.ToTable("TblDefConsultant", "dbikduser");

            entity.Property(e => e.CompanyId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("Company_ID");
            entity.Property(e => e.Approve)
                .HasDefaultValue(false)
                .HasColumnName("approve");
            entity.Property(e => e.CityId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("city_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fax)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsInquiry).HasColumnName("is_inquiry");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Logo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("logo");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.MaillingAddress)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MemberId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Photo1)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("photo1");
            entity.Property(e => e.Photo2)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("photo2");
            entity.Property(e => e.Photo3)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("photo3");
            entity.Property(e => e.PremiumDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("premium_details");
            entity.Property(e => e.PremiumMember)
                .HasDefaultValue(false)
                .HasColumnName("premium_member");
            entity.Property(e => e.RewriteUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("rewrite_url");
            entity.Property(e => e.Serviceprovide)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("serviceprovide");
            entity.Property(e => e.Views)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("views");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.YearEstablished)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Zoom).HasColumnName("zoom");
        });

        modelBuilder.Entity<TblDefCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK_tblDefCountry");

            entity.ToTable("TblDefCountry", "dbikduser");

            entity.Property(e => e.CountryId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("country_name");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<TblDefMemberInfo2>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__TblDefMe__0CF04B1886261B21");

            entity.ToTable("TblDefMemberInfo2", "dbikduser");

            entity.Property(e => e.MemberId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CityId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.InstId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.ProgramOfStudy).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Updated).HasColumnType("datetime");
            entity.Property(e => e.Views).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<TblDefMemberInfoikd>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__TblDefMe__0CF04B18130288B0");

            entity.ToTable("TblDefMemberInfoikd", "dbikduser");

            entity.Property(e => e.MemberId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CityId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fbimage)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Fburl).HasMaxLength(500);
            entity.Property(e => e.Id).HasMaxLength(255);
            entity.Property(e => e.ImageName).HasMaxLength(255);
            entity.Property(e => e.InstId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.MemberFirstName).HasMaxLength(255);
            entity.Property(e => e.MemberLastName).HasMaxLength(255);
            entity.Property(e => e.MemberName).HasMaxLength(255);
            entity.Property(e => e.MemberSource).HasMaxLength(255);
            entity.Property(e => e.MobileNumber).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Passwordold).HasMaxLength(255);
            entity.Property(e => e.ProgramOfStudy).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RewriteUrl).HasMaxLength(255);
            entity.Property(e => e.Thumbnail).HasMaxLength(255);
            entity.Property(e => e.Updated).HasColumnType("datetime");
            entity.Property(e => e.VerificationCode).HasMaxLength(255);
            entity.Property(e => e.Views).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<TblDefSubject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);

            entity.ToTable("tblDefSubjects", "dbo");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.IsGeneral).HasColumnName("is_general");
            entity.Property(e => e.SearchTags)
                .HasMaxLength(500)
                .HasColumnName("search_tags");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.SubjectIcon).HasColumnName("subject_icon");
            entity.Property(e => e.SubjectName).HasColumnName("subject_name");
        });

        modelBuilder.Entity<TblEducationLevel>(entity =>
        {
            entity.HasKey(e => e.EducationLevelId);

            entity.ToTable("tblEducationLevels", "dbo");

            entity.Property(e => e.EducationLevelId).HasColumnName("education_level_id");
            entity.Property(e => e.Courselevel).HasColumnName("courselevel");
            entity.Property(e => e.EducationLevelName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("education_level_name");
            entity.Property(e => e.EducationLevelNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("education_level_no");
            entity.Property(e => e.Isacademic).HasColumnName("isacademic");
            entity.Property(e => e.LevelImage).HasColumnName("level_image");
            entity.Property(e => e.SearchTags)
                .HasMaxLength(500)
                .HasColumnName("search_tags");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<TblFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Files__3214EC07ECAF3A77");

            entity.ToTable("TblFiles", "dbikduser");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Section).WithMany(p => p.TblFiles)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Files_Sections");
        });

        modelBuilder.Entity<TblFlag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Flagss");

            entity.ToTable("TblFlags", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Neighbour).HasColumnName("neighbour");
        });

        modelBuilder.Entity<TblGuide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Guide");

            entity.ToTable("TblGuide", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.QualificationId).HasColumnName("QualificationID");
        });

        modelBuilder.Entity<TblGuideDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_GuideDetail");

            entity.ToTable("TblGuideDetail", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GuideId).HasColumnName("GuideID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsMenu).HasDefaultValue(true);
            entity.Property(e => e.PageId).HasColumnName("PageID");
        });

        modelBuilder.Entity<TblGuidesDefination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblGuide__3214EC0770A0651D");

            entity.ToTable("TblGuidesDefination", "dbikduser");

            entity.Property(e => e.Abrevation).HasMaxLength(100);
            entity.Property(e => e.CategoryIds).HasMaxLength(500);
            entity.Property(e => e.CourseUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
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

        modelBuilder.Entity<TblHostel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblHostel_Rooms");

            entity.ToTable("tblHostels", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityAreaId).HasColumnName("CityAreaID");
            entity.Property(e => e.CityAreaName).HasMaxLength(150);
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.FeatureIds).HasMaxLength(250);
            entity.Property(e => e.FullAddress).HasMaxLength(1000);
            entity.Property(e => e.HostelDetails).HasMaxLength(1000);
            entity.Property(e => e.HostelName).HasMaxLength(250);
            entity.Property(e => e.Latitude)
                .HasMaxLength(100)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(100)
                .HasColumnName("longitude");
            entity.Property(e => e.MemberId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("MemberID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.RewriteUrl).HasColumnName("rewrite_url");
            entity.Property(e => e.RoomHeading).HasMaxLength(200);
            entity.Property(e => e.Zoomlevel)
                .HasMaxLength(5)
                .HasColumnName("zoomlevel");
        });

        modelBuilder.Entity<TblHostelFacility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblHoste__3214EC2715E9A4E0");

            entity.ToTable("tblHostelFacility", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<TblHostelFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId);

            entity.ToTable("tblHostelFeatures", "dbo");

            entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
            entity.Property(e => e.FeatureName).HasMaxLength(100);
            entity.Property(e => e.IconClass).HasMaxLength(50);
        });

        modelBuilder.Entity<TblHostelImage>(entity =>
        {
            entity.ToTable("tblHostelImages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.HostelId).HasColumnName("HostelID");
            entity.Property(e => e.ImageName).HasColumnName("image_name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Thumbnail).HasColumnName("thumbnail");

            entity.HasOne(d => d.Hostel).WithMany(p => p.TblHostelImages)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK_tblHostelImages_tblHostels");
        });

        modelBuilder.Entity<TblHostelRoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId);

            entity.ToTable("tblHostelRoomTypes", "dbo");

            entity.Property(e => e.RoomTypeId).HasColumnName("RoomTypeID");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DetailsRoomType).HasMaxLength(250);
            entity.Property(e => e.HostelId).HasColumnName("HostelID");
            entity.Property(e => e.RoomType).HasMaxLength(150);

            entity.HasOne(d => d.Hostel).WithMany(p => p.TblHostelRoomTypes)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK_tblHostelRoomTypes_tblHostels");
        });

        modelBuilder.Entity<TblInstitute>(entity =>
        {
            entity.HasKey(e => e.InstId).HasName("PK__TblInsti__E2A29686FDDB3FD6");

            entity.ToTable("TblInstitutes", "dbikduser");

            entity.Property(e => e.Abbrevation).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Backgroundimage).HasMaxLength(500);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Datesheeturl).HasMaxLength(500);
            entity.Property(e => e.DegreeStatus).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Established).HasMaxLength(100);
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.Fbranking).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FemaleStudents).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GenderType).HasMaxLength(50);
            entity.Property(e => e.GoogleRanking).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InstCategy).HasMaxLength(255);
            entity.Property(e => e.InstLogoPath).HasMaxLength(500);
            entity.Property(e => e.InstName).HasMaxLength(255);
            entity.Property(e => e.Joburl).HasMaxLength(500);
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.Property(e => e.MaleStudents).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.PageTitle).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.ProspectusUrl).HasMaxLength(500);
            entity.Property(e => e.Qsranking).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Resulturl).HasMaxLength(500);
            entity.Property(e => e.RewriteUrl).HasMaxLength(255);
            entity.Property(e => e.Theranking).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.Views).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Website).HasMaxLength(255);
            entity.Property(e => e.Zoomlevel).HasMaxLength(50);
        });

        modelBuilder.Entity<TblLecture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC274EE52181");

            entity.ToTable("tblLectures", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Duration).HasMaxLength(50);
            entity.Property(e => e.IsDelete).HasDefaultValue(true);
            entity.Property(e => e.LectureChId).HasColumnName("LectureChID");
            entity.Property(e => e.LectureClassId).HasColumnName("LectureClassID");
            entity.Property(e => e.LectureName).HasMaxLength(255);
            entity.Property(e => e.LectureSubjectId).HasColumnName("LectureSubjectID");
            entity.Property(e => e.LectureTeacherId).HasColumnName("LectureTeacherID");
            entity.Property(e => e.LectureThumb)
                .HasMaxLength(255)
                .HasColumnName("LectureTHumb");
            entity.Property(e => e.LectureTopicId).HasColumnName("LectureTopicID");
            entity.Property(e => e.MetaKeys).HasMaxLength(500);
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LectureCh).WithMany(p => p.TblLectures)
                .HasForeignKey(d => d.LectureChId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__10966653");

            entity.HasOne(d => d.LectureClass).WithMany(p => p.TblLectures)
                .HasForeignKey(d => d.LectureClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__0EAE1DE1");

            entity.HasOne(d => d.LectureSubject).WithMany(p => p.TblLectures)
                .HasForeignKey(d => d.LectureSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__0FA2421A");

            entity.HasOne(d => d.LectureTeacher).WithMany(p => p.TblLectures)
                .HasForeignKey(d => d.LectureTeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__127EAEC5");

            entity.HasOne(d => d.LectureTopic).WithMany(p => p.TblLectures)
                .HasForeignKey(d => d.LectureTopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__118A8A8C");
        });

        modelBuilder.Entity<TblLectureChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC271A38001F");

            entity.ToTable("tblLectureChapters", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChImage).HasMaxLength(255);
            entity.Property(e => e.ChName).HasMaxLength(255);
            entity.Property(e => e.Chtitle).HasMaxLength(255);
            entity.Property(e => e.LectureClassId).HasColumnName("LectureClassID");
            entity.Property(e => e.LectureSubjectId).HasColumnName("LectureSubjectID");
            entity.Property(e => e.Metadesc).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LectureClass).WithMany(p => p.TblLectureChapters)
                .HasForeignKey(d => d.LectureClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__04308F6E");

            entity.HasOne(d => d.LectureSubject).WithMany(p => p.TblLectureChapters)
                .HasForeignKey(d => d.LectureSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__0524B3A7");
        });

        modelBuilder.Entity<TblLectureClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC2723BBA22C");

            entity.ToTable("tblLectureClasses", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClassName).HasMaxLength(255);
            entity.Property(e => e.ClassUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GuideCourseId).HasColumnName("GuideCourseID");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblLectureSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC278792C8DA");

            entity.ToTable("tblLectureSubject", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.LectureClassId).HasColumnName("LectureClassID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LectureClass).WithMany(p => p.TblLectureSubjects)
                .HasForeignKey(d => d.LectureClassId)
                .HasConstraintName("FK__tblLectur__Lectu__79B300FB");
        });

        modelBuilder.Entity<TblLectureTeacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC2771970395");

            entity.ToTable("tblLectureTeacher", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblLectureTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblLectu__3214EC271F22ABDB");

            entity.ToTable("tblLectureTopics", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LectureChId).HasColumnName("LectureChID");
            entity.Property(e => e.LectureClassId).HasColumnName("LectureClassID");
            entity.Property(e => e.LectureSubjectId).HasColumnName("LectureSubjectID");
            entity.Property(e => e.TopicName).HasMaxLength(255);
            entity.Property(e => e.TopicNumber).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LectureCh).WithMany(p => p.TblLectureTopics)
                .HasForeignKey(d => d.LectureChId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__0ADD8CFD");

            entity.HasOne(d => d.LectureClass).WithMany(p => p.TblLectureTopics)
                .HasForeignKey(d => d.LectureClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__08F5448B");

            entity.HasOne(d => d.LectureSubject).WithMany(p => p.TblLectureTopics)
                .HasForeignKey(d => d.LectureSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblLectur__Lectu__09E968C4");
        });

        modelBuilder.Entity<TblLongQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__tblLongQ__2EC21549EB5B2A89");

            entity.ToTable("tblLongQuestions", "dbo");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.ChapterId).HasColumnName("chapter_id");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.QuestionDescription).HasColumnName("question_description");
            entity.Property(e => e.QuestionImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("question_image");
            entity.Property(e => e.QuestionMarks)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("question_marks");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");

            entity.HasOne(d => d.User).WithMany(p => p.TblLongQuestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblLongQuestions_Admin");
        });

        modelBuilder.Entity<TblLongQuestionAnswerChoice>(entity =>
        {
            entity.HasKey(e => e.ChoiceId).HasName("PK__tblLongQ__33CAF83A73423758");

            entity.ToTable("tblLongQuestionAnswerChoices", "dbo");

            entity.Property(e => e.ChoiceId).HasColumnName("choice_id");
            entity.Property(e => e.ChoiceDescription).HasColumnName("choice_description");
            entity.Property(e => e.ChoiceImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("choice_image");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
        });

        modelBuilder.Entity<TblMainNews>(entity =>
        {
            entity.HasKey(e => e.NewsId).IsClustered(false);

            entity.ToTable("tblMainNews", "dbo");

            entity.Property(e => e.NewsId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("news_id");
            entity.Property(e => e.Approve)
                .HasDefaultValue(false)
                .HasColumnName("approve");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.MainHeading)
                .HasMaxLength(250)
                .HasDefaultValue(" ")
                .HasColumnName("main_heading");
            entity.Property(e => e.MemberId)
                .HasDefaultValue(13148m)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(500)
                .HasColumnName("meta_title");
            entity.Property(e => e.NewsDescription)
                .HasMaxLength(500)
                .HasColumnName("news_description");
            entity.Property(e => e.NewsDetails).HasColumnName("news_details");
            entity.Property(e => e.NewsKeywords)
                .HasMaxLength(500)
                .HasColumnName("news_keywords");
            entity.Property(e => e.NewsQuote)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("news_quote");
            entity.Property(e => e.NewsSource)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("news_source");
            entity.Property(e => e.NewsTags)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("news_tags");
            entity.Property(e => e.OtherFile)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("other_file");
            entity.Property(e => e.Picture1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_1");
            entity.Property(e => e.Picture1Desc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_1_desc");
            entity.Property(e => e.Picture2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_2");
            entity.Property(e => e.Picture2Desc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_2_desc");
            entity.Property(e => e.Picture3)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_3");
            entity.Property(e => e.Picture3Desc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_3_desc");
            entity.Property(e => e.PictureStoryId).HasColumnName("picture_story_id");
            entity.Property(e => e.PictureThumbnail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("picture_thumbnail");
            entity.Property(e => e.ResultKeywords).HasMaxLength(500);
            entity.Property(e => e.RewriteUrl)
                .HasMaxLength(500)
                .HasColumnName("rewrite_url");
            entity.Property(e => e.ShortDesc)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("short_desc");
            entity.Property(e => e.ShortHeading)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("short_heading");
            entity.Property(e => e.SubHeading)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("sub_heading");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
            entity.Property(e => e.Views)
                .HasDefaultValue(10m)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("views");

            entity.HasOne(d => d.User).WithMany(p => p.TblMainNews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tblMainNews_Admin");
        });

        modelBuilder.Entity<TblMemberTestHistory2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TblMemberTestHistory2", "dbikduser");

            entity.Property(e => e.MemberId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ObtainedMarks).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TestDuration).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TblMeritList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblMerit__3214EC278A9805D8");

            entity.ToTable("tblMeritList", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
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
            entity.HasKey(e => e.Id).HasName("PK__TblMerit__3214EC270AB00C34");

            entity.ToTable("TblMeritListType", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MeritListTypeName).IsUnicode(false);
        });

        modelBuilder.Entity<TblNewsCategory>(entity =>
        {
            entity.HasKey(e => e.NewsCategoryId).HasName("PK__NewNewsC__01154D78DE66C97A");

            entity.ToTable("tblNewsCategories", "dbo");

            entity.Property(e => e.NewsCategoryId).HasColumnName("news_category_id");
            entity.Property(e => e.CategoryOrder).HasColumnName("category_order");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.NewsCategoryName)
                .HasMaxLength(255)
                .HasColumnName("news_category_name");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.RewriteUrl)
                .HasMaxLength(255)
                .HasColumnName("rewrite_url");
        });

        modelBuilder.Entity<TblNewsCollege>(entity =>
        {
            entity.HasKey(e => e.NcId);

            entity.ToTable("tblNewsColleges", "dbo");

            entity.Property(e => e.NcId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("nc_id");
            entity.Property(e => e.InstId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("inst_id");
            entity.Property(e => e.NewsId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("news_id");
        });

        modelBuilder.Entity<TblNewsComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("tblNewsComments", "dbo");

            entity.Property(e => e.CommentId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("comment_id");
            entity.Property(e => e.Comments)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("comments");
            entity.Property(e => e.DatePosted)
                .HasColumnType("smalldatetime")
                .HasColumnName("date_posted");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved");
            entity.Property(e => e.MemberImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("member_image");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NewsId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("news_id");
            entity.Property(e => e.PageUrl)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("page_url");
        });

        modelBuilder.Entity<TblNewsLetter>(entity =>
        {
            entity.HasKey(e => e.NlId);

            entity.ToTable("tblNewsLetter", "dbo");

            entity.Property(e => e.NlId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("nl_id");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.NlActive)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nl_active");
            entity.Property(e => e.NlEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nl_email");
        });

        modelBuilder.Entity<TblNewsLink>(entity =>
        {
            entity.HasKey(e => e.LinkId);

            entity.ToTable("tblNewsLinks", "dbo");

            entity.Property(e => e.LinkId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("link_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.NewsId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("news_id");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("url");
        });

        modelBuilder.Entity<TblNewsMultiCategory>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("tblNewsMultiCategories", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NewsCategoryId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("news_category_id");
            entity.Property(e => e.NewsId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("news_id");
        });

        modelBuilder.Entity<TblOtsChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblOtsCh__3214EC07F89D1CFD");

            entity.ToTable("TblOtsChapter", "dbikduser");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OtschName).HasColumnName("OTSChName");
            entity.Property(e => e.OtschNo).HasColumnName("OTSChNo");
            entity.Property(e => e.OtschUrl).HasColumnName("OTSChURL");
            entity.Property(e => e.OtsclassId).HasColumnName("OTSClassID");
            entity.Property(e => e.OtssubjectId).HasColumnName("OTSSubjectID");
        });

        modelBuilder.Entity<TblOtsTestCriteriaDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblOtsTe__3214EC071441C9EA");

            entity.ToTable("TblOtsTestCriteriaDetail", "dbikduser");
        });

        modelBuilder.Entity<TblOtsTestCriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblOtsmc__3214EC074D4E90DE");

            entity.ToTable("TblOtsTestCriteria", "dbikduser");

            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblOtsTestMcq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblOtsQu__3214EC07D2D9A642");

            entity.ToTable("TblOtsTestMcq", "dbikduser");

            entity.Property(e => e.Choice1img).HasMaxLength(255);
            entity.Property(e => e.Choice2img).HasMaxLength(255);
            entity.Property(e => e.Choice3img).HasMaxLength(255);
            entity.Property(e => e.Choice4img).HasMaxLength(255);
            entity.Property(e => e.Choice5img).HasMaxLength(255);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.QuestionImage).HasMaxLength(255);
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");

            entity.HasOne(d => d.User).WithMany(p => p.TblOtsTestMcqs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TblOtsTestMcq_Admin");
        });

        modelBuilder.Entity<TblOtsTopic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__TblOtsTo__022E0F5D0718EE62");

            entity.ToTable("TblOtsTopics", "dbikduser");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.TopicName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TopicNumber).HasColumnType("decimal(10, 1)");
        });

        modelBuilder.Entity<TblOtsclass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSCl__3214EC275047C6AE");

            entity.ToTable("tblOTSClass", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.OtsclassName)
                .HasMaxLength(255)
                .HasColumnName("OTSClassName");
            entity.Property(e => e.OtsclassUrl).HasColumnName("OTSClassURL");
        });

        modelBuilder.Entity<TblOtsmcqchild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSMC__3214EC277CF9EB35");

            entity.ToTable("tblOTSMCQChild", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Choice1).HasColumnName("CHOICE1");
            entity.Property(e => e.Choice1img).HasColumnName("CHOICE1Img");
            entity.Property(e => e.Choice2).HasColumnName("CHOICE2");
            entity.Property(e => e.Choice2img).HasColumnName("CHOICE2Img");
            entity.Property(e => e.Choice3).HasColumnName("CHOICE3");
            entity.Property(e => e.Choice3img).HasColumnName("CHOICE3Img");
            entity.Property(e => e.Choice4).HasColumnName("CHOICE4");
            entity.Property(e => e.Choice4img).HasColumnName("CHOICE4Img");
            entity.Property(e => e.Choice5).HasColumnName("CHOICE5");
            entity.Property(e => e.Choice5img).HasColumnName("CHOICE5Img");
            entity.Property(e => e.CorrectAnswer).HasColumnName("CORRECT_ANSWER");
            entity.Property(e => e.Otsmcqid).HasColumnName("OTSMCQID");
            entity.Property(e => e.Question).HasColumnName("QUESTION");
        });

        modelBuilder.Entity<TblOtsquiz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSQu__3213E83FC07966BC");

            entity.ToTable("tblOTSQuiz", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.OtsquizCatId).HasColumnName("OTSQuizCatID");
            entity.Property(e => e.QuizName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblOtsquizCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSQu__3214EC27362C25F2");

            entity.ToTable("tblOTSQuizCategory", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OtsquizCategoryName)
                .HasMaxLength(255)
                .HasColumnName("OTSQuizCategoryName");
            entity.Property(e => e.Url)
                .HasMaxLength(2083)
                .HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblOtsquizChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSQu__3214EC2720900990");

            entity.ToTable("tblOTSQuizChild", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblOtssubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblOTSSu__3214EC07B541EC21");

            entity.ToTable("tblOTSSubject", "dbikduser");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TblPageFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblPageF__3214EC07BE7615AF");

            entity.ToTable("TblPageFeedback", "dbikduser");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.PageSection).HasMaxLength(250);
            entity.Property(e => e.PageUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<TblPastPaper>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPastP__3214EC27A573DD75");

            entity.ToTable("tblPastPapers", "dbikduser");

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
                .HasConstraintName("FK__tblPastPa__Board__15C52FC4");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPastPapers)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPastPa__PPCla__16B953FD");

            entity.HasOne(d => d.Ppsubject).WithMany(p => p.TblPastPapers)
                .HasForeignKey(d => d.PpsubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPastPa__PPSub__17AD7836");
        });

        modelBuilder.Entity<TblPastPaperType>(entity =>
        {
            entity.ToTable("TblPastPaperType", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.PastPaperId).HasColumnName("PastPaperID");
            entity.Property(e => e.PastPaperTypeId).HasColumnName("PastPaperTypeID");
        });

        modelBuilder.Entity<TblPlaceOfStudy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PlaceOfStudy");

            entity.ToTable("TblPlaceOfStudy", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ikdurl).HasColumnName("IKDUrl");
        });

        modelBuilder.Entity<TblPpboardClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPBoa__3214EC278DE8202A");

            entity.ToTable("tblPPBoardClasses", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.PpclassId).HasColumnName("PPClassID");

            entity.HasOne(d => d.Board).WithMany(p => p.TblPpboardClasses)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__Board__058EC7FB");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPpboardClasses)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__PPCla__0682EC34");
        });

        modelBuilder.Entity<TblPpboardClassSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPBoa__3214EC27D2B4069D");

            entity.ToTable("tblPPBoardClassSubjects", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.PpclassId).HasColumnName("PPClassID");
            entity.Property(e => e.PpsubjectId).HasColumnName("PPSubjectID");

            entity.HasOne(d => d.Board).WithMany(p => p.TblPpboardClassSubjects)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__Board__095F58DF");

            entity.HasOne(d => d.Ppclass).WithMany(p => p.TblPpboardClassSubjects)
                .HasForeignKey(d => d.PpclassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__PPCla__0A537D18");

            entity.HasOne(d => d.Ppsubject).WithMany(p => p.TblPpboardClassSubjects)
                .HasForeignKey(d => d.PpsubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblPPBoar__PPSub__0B47A151");
        });

        modelBuilder.Entity<TblPpclass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPCla__3214EC27FA596413");

            entity.ToTable("tblPPClass", "dbikduser");

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
                .HasConstraintName("FK__tblPPClas__PPQua__7FD5EEA5");
        });

        modelBuilder.Entity<TblPpqualification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPQua__3214EC273AC14555");

            entity.ToTable("tblPPQualification", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PpqualificationName).HasColumnName("PPQualificationName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblPpsubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPSub__3214EC272C6C6B48");

            entity.ToTable("tblPPSubject", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PpsubjectName).HasColumnName("PPSubjectName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblPptype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPPTyp__3214EC2702C4A2AB");

            entity.ToTable("tblPPTypes", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PptypeName)
                .HasMaxLength(255)
                .HasColumnName("PPTypeName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblResul__3214EC07C3E94284");

            entity.ToTable("TblResults", "dbikduser");

            entity.HasIndex(e => e.ResultUrl, "UQ__TblResul__5F8EF04FA125C9AD").IsUnique();

            entity.Property(e => e.AnnounceDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MetaDescription).HasMaxLength(300);
            entity.Property(e => e.MetaTitle).HasMaxLength(150);
            entity.Property(e => e.OfficialLink).HasMaxLength(250);
            entity.Property(e => e.ResultStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ResultTitle).HasMaxLength(250);
            entity.Property(e => e.ResultUrl).HasMaxLength(250);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblResultCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblResul__3214EC074AA3A605");

            entity.ToTable("TblResultCategory", "dbikduser");

            entity.HasIndex(e => e.Slug, "UQ__TblResul__BC7B5FB6E8AB3C89").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<TblResultClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblResul__3214EC078367C3E9");

            entity.ToTable("TblResultClasses", "dbikduser");

            entity.HasIndex(e => e.Slug, "UQ__TblResul__BC7B5FB6DE5DE2DC").IsUnique();

            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<TblResultCriteriaChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ResultArchive");

            entity.ToTable("TblResultCriteriaChild", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateText).HasMaxLength(50);
            entity.Property(e => e.ResultCriteriaId).HasColumnName("ResultCriteriaID");
            entity.Property(e => e.ResultGazette).HasMaxLength(300);
            entity.Property(e => e.RewardHtml).IsUnicode(false);
            entity.Property(e => e.ShortDesc).IsUnicode(false);
            entity.Property(e => e.WebsiteSource).HasMaxLength(500);
        });

        modelBuilder.Entity<TblResultCriterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ResultCriteria");

            entity.ToTable("TblResultCriteria", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.ResultShortCode).HasMaxLength(100);
            entity.Property(e => e.Show).HasColumnName("show");
        });

        modelBuilder.Entity<TblResultUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblResul__3214EC0748F3B8B8");

            entity.ToTable("TblResultUpdate", "dbikduser");

            entity.Property(e => e.ClassName).HasMaxLength(255);
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.MobileNo).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<TblSch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSch__3214EC2711BB8D09");

            entity.ToTable("tblSch", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Country).WithMany(p => p.TblSches)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_tblSch_Country");
        });

        modelBuilder.Entity<TblSchFieldofStudy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchFi__3214EC27BD986974");

            entity.ToTable("tblSchFieldofStudy", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblSchFieldsofStudyChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchFi__3214EC27F0024B5F");

            entity.ToTable("TblSchFieldsofStudyChild", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SchFieldofStudyId).HasColumnName("SchFieldofStudyID");
            entity.Property(e => e.SchId).HasColumnName("SchID");

            entity.HasOne(d => d.Sch).WithMany(p => p.TblSchFieldsofStudyChildren)
                .HasForeignKey(d => d.SchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSchFieldsofStudy_Sch");
        });

        modelBuilder.Entity<TblSchStudyLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchSt__3214EC275BAC9E82");

            entity.ToTable("tblSchStudyLevel", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblSchStudyLevelChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchSt__3214EC27B724A72D");

            entity.ToTable("tblSchStudyLevelChild", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SchId).HasColumnName("SchID");
            entity.Property(e => e.SchStudyLevelId).HasColumnName("SchStudyLevelID");

            entity.HasOne(d => d.Sch).WithMany(p => p.TblSchStudyLevelChildren)
                .HasForeignKey(d => d.SchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSchStudyLevels_Sch");

            entity.HasOne(d => d.SchStudyLevel).WithMany(p => p.TblSchStudyLevelChildren)
                .HasForeignKey(d => d.SchStudyLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSchStudyLevels_StudyLevel");
        });

        modelBuilder.Entity<TblSchTop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchTo__3214EC27E86EEF15");

            entity.ToTable("tblSchTop", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Country).WithMany(p => p.TblSchTops)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_tblSchTop_Country");
        });

        modelBuilder.Entity<TblSchTopLinking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSchTo__3214EC27994F1A4A");

            entity.ToTable("tblSchTopLinking", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SchId).HasColumnName("SchID");
            entity.Property(e => e.TopSchId).HasColumnName("TopSchID");

            entity.HasOne(d => d.Sch).WithMany(p => p.TblSchTopLinkings)
                .HasForeignKey(d => d.SchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSchTopLinking_Sch");

            entity.HasOne(d => d.TopSch).WithMany(p => p.TblSchTopLinkings)
                .HasForeignKey(d => d.TopSchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSchTopLinking_TopSch");
        });

        modelBuilder.Entity<TblSchWithCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ScholarShipCategories");

            entity.ToTable("TblSchWithCategory", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ScholarshipId).HasColumnName("ScholarshipID");
        });

        modelBuilder.Entity<TblSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSecti__3214EC27E899985B");

            entity.ToTable("tblSections", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cmsurlpattern).HasColumnName("CMSURLPattern");
            entity.Property(e => e.Url).HasColumnName("URL");
        });

        modelBuilder.Entity<TblShortQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__tblShort__2EC21549514D552D");

            entity.ToTable("tblShortQuestions", "dbo");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.ChapterId).HasColumnName("chapter_id");
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.QappeardYrs)
                .HasMaxLength(250)
                .HasColumnName("QAppeardYrs");
            entity.Property(e => e.QuestionDescription).HasColumnName("question_description");
            entity.Property(e => e.QuestionImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("question_image");
            entity.Property(e => e.QuestionMarks)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("question_marks");
        });

        modelBuilder.Entity<TblShortQuestionAnswerChoice>(entity =>
        {
            entity.HasKey(e => e.ChoiceId).HasName("PK__tblShort__33CAF83AD57AC212");

            entity.ToTable("tblShortQuestionAnswerChoices", "dbo");

            entity.Property(e => e.ChoiceId).HasColumnName("choice_id");
            entity.Property(e => e.ChoiceDescription).HasColumnName("choice_description");
            entity.Property(e => e.ChoiceImage)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("choice_image");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
        });

        modelBuilder.Entity<TblSlide>(entity =>
        {
            entity.ToTable("tblSlide", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.MainImage).HasMaxLength(500);
            entity.Property(e => e.SliderDesc).HasMaxLength(2000);
            entity.Property(e => e.SliderId).HasColumnName("SliderID");
            entity.Property(e => e.SliderTitle).HasMaxLength(500);
        });

        modelBuilder.Entity<TblSlide1>(entity =>
        {
            entity.ToTable("tblSlides", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Link).IsUnicode(false);
            entity.Property(e => e.Price).HasMaxLength(50);
            entity.Property(e => e.SlideName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Slide_Name");
            entity.Property(e => e.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title1).HasMaxLength(250);
            entity.Property(e => e.Title2).HasMaxLength(250);
        });

        modelBuilder.Entity<TblSlider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSlide__3213E83F66CEEECB");

            entity.ToTable("tblSliders", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Isbanned).HasColumnName("isbanned");
            entity.Property(e => e.Sortorder).HasColumnName("sortorder");
            entity.Property(e => e.Thumbnail).HasColumnName("thumbnail");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<TblSliderCategory>(entity =>
        {
            entity.ToTable("tblSliderCategory", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IconImage).HasMaxLength(500);
            entity.Property(e => e.SliderCategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<TblStudyAbroadGuide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblStudy__3214EC07CC9AE797");

            entity.ToTable("TblStudyAbroadGuides", "dbikduser");

            entity.HasOne(d => d.Country).WithMany(p => p.TblStudyAbroadGuides)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TblStudyA__Count__7B4643B2");
        });

        modelBuilder.Entity<TblStudyGuide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblStudy__3214EC2738DD8E58");

            entity.ToTable("tblStudyGuides", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.EduLevelId).HasColumnName("EduLevelID");
            entity.Property(e => e.ShortName).HasMaxLength(100);
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Board).WithMany(p => p.TblStudyGuides)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_tblStudyGuides_Board");

            entity.HasOne(d => d.EduLevel).WithMany(p => p.TblStudyGuides)
                .HasForeignKey(d => d.EduLevelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_tblStudyGuides_EduLevel");
        });

        modelBuilder.Entity<TblSubject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);

            entity.ToTable("tblSubject", "dbo");

            entity.Property(e => e.SubjectId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("subject_Id");
            entity.Property(e => e.ClassId).HasColumnName("class_Id");
            entity.Property(e => e.ImageName).IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.SubjectDuration)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subject_duration");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("subject_Name");
            entity.Property(e => e.SubjectTotalMarks)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subject_total_marks");
            entity.Property(e => e.SubjectTotalQuestions).HasColumnName("subject_total_questions");
            entity.Property(e => e.SubjectUrl).HasColumnName("subjectUrl");
        });

        modelBuilder.Entity<TblTeacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblTeach__3214EC27CBC6C80B");

            entity.ToTable("tblTeachers", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

        modelBuilder.Entity<TblTest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblTest__3214EC273F5C9F73");

            entity.ToTable("TblTest", "dbikduser");

            entity.HasIndex(e => e.TestId, "IX_TblTest_TestId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.IsBrowser).HasColumnName("isBrowser");
            entity.Property(e => e.IsPassed).HasColumnName("isPassed");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ObtainedMarks).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblTltutor>(entity =>
        {
            entity.HasKey(e => e.TlTutorId);

            entity.ToTable("tblTLTutor", "dbo");

            entity.Property(e => e.TlTutorId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("tl_tutor_id");
            entity.Property(e => e.AboutTutoring)
                .IsUnicode(false)
                .HasColumnName("about_tutoring");
            entity.Property(e => e.ActiveMailAlert)
                .HasDefaultValue(true)
                .HasComment("1 for active alert and 0 for disable alert")
                .HasColumnName("active_mail_alert");
            entity.Property(e => e.Availability)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("availability");
            entity.Property(e => e.CourseName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Dated)
                .HasColumnType("datetime")
                .HasColumnName("dated");
            entity.Property(e => e.Experience)
                .IsUnicode(false)
                .HasColumnName("experience");
            entity.Property(e => e.IsVerified).HasColumnName("isVerified");
            entity.Property(e => e.MemberId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("member_id");
            entity.Property(e => e.TutoringOptions)
                .HasMaxLength(50)
                .HasColumnName("tutoringOptions");
        });

        modelBuilder.Entity<TblTutorFavourite>(entity =>
        {
            entity.HasKey(e => e.FavId);

            entity.ToTable("tblTutorFavourite", "dbo");

            entity.Property(e => e.FavId).HasColumnName("fav_Id");
            entity.Property(e => e.MemberId).HasColumnName("member_Id");
            entity.Property(e => e.TutorId).HasColumnName("tutor_id");
        });

        modelBuilder.Entity<TblTutorLevel>(entity =>
        {
            entity.ToTable("tblTutorLevels", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EducationLevelId).HasColumnName("education_level_id");
            entity.Property(e => e.TlTutorId).HasColumnName("tl_tutor_id");
        });

        modelBuilder.Entity<TblTutorMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblTutor__3214EC279E0F037D");

            entity.ToTable("tblTutorMessages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AttachmentThumb).HasMaxLength(500);
            entity.Property(e => e.ConversationId).HasColumnName("ConversationID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");
        });

        modelBuilder.Entity<TblTutorSubject>(entity =>
        {
            entity.ToTable("tblTutorSubjects", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TlTutorId).HasColumnName("tl_tutor_id");
        });

        modelBuilder.Entity<TblUrlcontentMigrate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblURLContent");

            entity.ToTable("TblURLContentMigrate", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content3).HasColumnName("content3");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.LastEdited).HasMaxLength(1000);
            entity.Property(e => e.MainImage).HasMaxLength(1000);
            entity.Property(e => e.MetaDescription).HasMaxLength(1000);
            entity.Property(e => e.MetaKeywords).HasMaxLength(1000);
            entity.Property(e => e.MetaTitle).HasMaxLength(1000);
            entity.Property(e => e.PageName).HasMaxLength(1000);
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<TblWhatsAppGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblWhats__3214EC07281FB5F9");

            entity.ToTable("TblWhatsAppGroups", "dbikduser");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupLink).HasMaxLength(500);
            entity.Property(e => e.GuideName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblXcourseLevel>(entity =>
        {
            entity.ToTable("tblXCourseLevels", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
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

        modelBuilder.Entity<Tblarticlestypemetadatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblarticlestypemetadata", "dbikduser");

            entity.Property(e => e.CaiId)
                .ValueGeneratedOnAdd()
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("cai_id");
            entity.Property(e => e.CatId).HasColumnName("cat_id");
            entity.Property(e => e.MetaDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("meta_description");
            entity.Property(e => e.MetaKeywords)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("meta_keywords");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Tbldefmembertype2>(entity =>
        {
            entity.HasKey(e => e.MemberTypeId).HasName("PK__Tbldefme__5D8AFD1FFB6BD909");

            entity.ToTable("Tbldefmembertype2", "dbikduser");

            entity.Property(e => e.MemberTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Tblpagewisecontent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PagewiseContent");

            entity.ToTable("tblpagewisecontent", "dbikduser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.FbHeader).HasMaxLength(2000);
            entity.Property(e => e.ImageName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetaDesc).HasMaxLength(600);
            entity.Property(e => e.MetaKeyword).HasMaxLength(600);
            entity.Property(e => e.MetaTags).HasMaxLength(600);
            entity.Property(e => e.MetaTitle).HasMaxLength(600);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PageLink)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.PageShortDetail).HasMaxLength(1000);
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.TxtDetailPopuler).HasColumnName("txtDetailPopuler");
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tblxinsttype>(entity =>
        {
            entity.HasKey(e => e.InstTypeId);

            entity.ToTable("tblxinsttype", "dbo");

            entity.Property(e => e.InstTypeId).HasColumnName("inst_type_id");
            entity.Property(e => e.InstTypeName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValue(" ")
                .HasColumnName("inst_type_name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<TestServiceProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestServ__3214EC27E542A361");

            entity.ToTable("TestServiceProvider", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<TestServiceProviderRelationWithJobAd>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestServ__3214EC27F941EF84");

            entity.ToTable("TestServiceProviderRelationWithJobAds", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Tspid).HasColumnName("TSPID");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topics__022E0F7DD97BCD89");

            entity.ToTable("Topics", "dbo");

            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.ChapterId).HasColumnName("ChapterID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.Date).HasColumnType("smalldatetime");
            entity.Property(e => e.TopicNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<TutorContactForm>(entity =>
        {
            entity.ToTable("TutorContactForm", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
        });

        modelBuilder.Entity<TutorDocument>(entity =>
        {
            entity.ToTable("TutorDocuments", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
        });

        modelBuilder.Entity<TutorInquiry>(entity =>
        {
            entity.ToTable("tutorInquiry", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Contact).HasColumnName("contact");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Detail).HasColumnName("detail");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsShared).HasColumnName("is_shared");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.QualificationId).HasColumnName("qualification_id");
            entity.Property(e => e.RequestType).HasColumnName("requestType");
            entity.Property(e => e.Subjects).HasColumnName("subjects");
            entity.Property(e => e.TutoringOptions).HasColumnName("tutoring_options");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0702919191");

            entity.ToTable("Users", "dbikduser");

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<WebStorySlider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblSlider");

            entity.ToTable("webStorySlider", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AuthorInfo).HasMaxLength(300);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.MainImage).HasMaxLength(500);
            entity.Property(e => e.SliderName).HasMaxLength(1000);
            entity.Property(e => e.SlierCategoryId).HasColumnName("SlierCategoryID");

            entity.HasOne(d => d.SlierCategory).WithMany(p => p.WebStorySliders)
                .HasForeignKey(d => d.SlierCategoryId)
                .HasConstraintName("FK_tblSlider_tblSliderCategory");
        });
        modelBuilder.HasSequence("MemberId_Seq", "dbikduser").StartsAt(2866404L);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
