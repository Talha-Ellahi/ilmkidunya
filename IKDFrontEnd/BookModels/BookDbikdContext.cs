using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.BookModels;

public partial class BookDbikdContext : DbContext
{
    public BookDbikdContext()
    {
    }

    public BookDbikdContext(DbContextOptions<BookDbikdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutU> AboutUs { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<BookDetail> BookDetails { get; set; }

    public virtual DbSet<BookOrder> BookOrders { get; set; }

    public virtual DbSet<BookRequest> BookRequests { get; set; }

    public virtual DbSet<BookReview> BookReviews { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<ContentPage> ContentPages { get; set; }

    public virtual DbSet<ContentUnit> ContentUnits { get; set; }

    public virtual DbSet<DliveryStatus> DliveryStatuses { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<Gallery> Galleries { get; set; }

    public virtual DbSet<GalleryCategory> GalleryCategories { get; set; }

    public virtual DbSet<GalleryPhoto> GalleryPhotos { get; set; }

    public virtual DbSet<HomePageImage> HomePageImages { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetail1> OrderDetails1 { get; set; }

    public virtual DbSet<OrderShippingDetail> OrderShippingDetails { get; set; }

    public virtual DbSet<PanelMenu> PanelMenus { get; set; }

    public virtual DbSet<PanelMenuLink> PanelMenuLinks { get; set; }

    public virtual DbSet<PanelMenuUserRelation> PanelMenuUserRelations { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBrand> ProductBrands { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductPhoto> ProductPhotos { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<RequestBook> RequestBooks { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<ShippingProductDetail> ShippingProductDetails { get; set; }

    public virtual DbSet<SocialMedium> SocialMedia { get; set; }

    public virtual DbSet<StoreOwner> StoreOwners { get; set; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<TblBanner> TblBanners { get; set; }

    public virtual DbSet<TblCity> TblCities { get; set; }

    public virtual DbSet<TblColor> TblColors { get; set; }

    public virtual DbSet<TblMember> TblMembers { get; set; }

    public virtual DbSet<TblPage> TblPages { get; set; }

    public virtual DbSet<TblProductColor> TblProductColors { get; set; }

    public virtual DbSet<TblSlider> TblSliders { get; set; }

    public virtual DbSet<TblTeamMember> TblTeamMembers { get; set; }

    public virtual DbSet<TblVideo> TblVideos { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<UrlbasedCmspage> UrlbasedCmspages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbbookstore");

        modelBuilder.Entity<AboutU>(entity =>
        {
            entity.ToTable("AboutUs", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BannerImage).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(500);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Authors", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.IsTop).HasColumnName("isTop");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books", "dbo");

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Binding).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.NumofPages).HasMaxLength(50);
            entity.Property(e => e.PublicationDate).HasMaxLength(50);
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.StoreId).HasColumnName("StoreID");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.ToTable("BookCategories", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<BookDetail>(entity =>
        {
            entity.ToTable("BookDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Binding).HasMaxLength(50);
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.NumofPages).HasMaxLength(50);
            entity.Property(e => e.PublicationDate).HasMaxLength(50);
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.StoreId).HasColumnName("StoreID");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<BookOrder>(entity =>
        {
            entity.ToTable("BookOrder", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Approve).HasColumnName("approve");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.ContactNo).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<BookRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookRequ__3214EC2772E80982");

            entity.ToTable("BookRequest", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.BookName).HasMaxLength(300);
            entity.Property(e => e.Contact).HasMaxLength(200);
            entity.Property(e => e.Edition).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(300);
        });

        modelBuilder.Entity<BookReview>(entity =>
        {
            entity.ToTable("BookReviews", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CatIcon).HasMaxLength(200);
            entity.Property(e => e.CategoryName).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(100);
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_Categories_Categories");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.ToTable("ContactUs", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmailAddress).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(500);
            entity.Property(e => e.Subject).HasMaxLength(500);
        });

        modelBuilder.Entity<ContentPage>(entity =>
        {
            entity.ToTable("ContentPages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.IsGuide).HasMaxLength(50);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(500);
            entity.Property(e => e.PageName).HasMaxLength(500);
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ContentPages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ContentPages_Users");
        });

        modelBuilder.Entity<ContentUnit>(entity =>
        {
            entity.ToTable("ContentUnit", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.UnitCode).HasMaxLength(50);
            entity.Property(e => e.UnitName).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ContentUnits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ContentUnit_Users");
        });

        modelBuilder.Entity<DliveryStatus>(entity =>
        {
            entity.ToTable("DliveryStatus", "dbo");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.ToTable("FAQs", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(500);
        });

        modelBuilder.Entity<Gallery>(entity =>
        {
            entity.ToTable("Gallery", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Url).HasMaxLength(250);
        });

        modelBuilder.Entity<GalleryCategory>(entity =>
        {
            entity.ToTable("GalleryCategory", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<GalleryPhoto>(entity =>
        {
            entity.ToTable("GalleryPhotos", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GalleryId).HasColumnName("GalleryID");
            entity.Property(e => e.Image).HasMaxLength(250);

            entity.HasOne(d => d.Gallery).WithMany(p => p.GalleryPhotos)
                .HasForeignKey(d => d.GalleryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GalleryPhotos_Gallery");
        });

        modelBuilder.Entity<HomePageImage>(entity =>
        {
            entity.ToTable("HomePageImages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Job", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.DeadLine).HasColumnType("datetime");
            entity.Property(e => e.NoOfPeople).HasMaxLength(50);
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.ToTable("JobApplication", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.Job).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_JobApplication_Job");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.ToTable("News", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(500);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order", "dbo");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ShippingPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.DliveryStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DliveryStatusId)
                .HasConstraintName("FK_Order_DliveryStatus");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_Order_tblMembers");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_Order_PaymentMode");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.StoreId).HasColumnName("StoreID");
        });

        modelBuilder.Entity<OrderDetail1>(entity =>
        {
            entity.ToTable("OrderDetails", "dbo");

            entity.Property(e => e.ColorId).HasColumnName("Color_id");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Color).WithMany(p => p.OrderDetail1s)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK__OrderDeta__Color__5FB337D6");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetail1s)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetails_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetail1s)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetails_Product");
        });

        modelBuilder.Entity<OrderShippingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OrderShippingDetail_ID");

            entity.ToTable("OrderShippingDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BlueExOrderCode).IsUnicode(false);
            entity.Property(e => e.Cncode)
                .IsUnicode(false)
                .HasColumnName("CNCode");
            entity.Property(e => e.CustomerAddress).IsUnicode(false);
            entity.Property(e => e.CustomerCity).IsUnicode(false);
            entity.Property(e => e.CustomerCityCode).IsUnicode(false);
            entity.Property(e => e.CustomerContactNo).IsUnicode(false);
            entity.Property(e => e.CustomerEmail).IsUnicode(false);
            entity.Property(e => e.CustomerName).IsUnicode(false);
            entity.Property(e => e.OrderPaymentType).IsUnicode(false);
            entity.Property(e => e.OrderReferenceCode).IsUnicode(false);
            entity.Property(e => e.OrderShippingPrice).IsUnicode(false);
            entity.Property(e => e.ShippingDate).HasColumnType("datetime");
            entity.Property(e => e.ShippingOriginCity).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
            entity.Property(e => e.TotalOrderAmount).IsUnicode(false);
        });

        modelBuilder.Entity<PanelMenu>(entity =>
        {
            entity.ToTable("PanelMenu", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<PanelMenuLink>(entity =>
        {
            entity.ToTable("PanelMenuLink", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PanelMenuId).HasColumnName("PanelMenuID");
            entity.Property(e => e.Title).HasMaxLength(500);

            entity.HasOne(d => d.PanelMenu).WithMany(p => p.PanelMenuLinks)
                .HasForeignKey(d => d.PanelMenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PanelMenuLink_PanelMenu");
        });

        modelBuilder.Entity<PanelMenuUserRelation>(entity =>
        {
            entity.ToTable("PanelMenuUserRelation", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PanelMenuId).HasColumnName("PanelMenuID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.PanelMenu).WithMany(p => p.PanelMenuUserRelations)
                .HasForeignKey(d => d.PanelMenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PanelMenuUserRelation_PanelMenu");

            entity.HasOne(d => d.User).WithMany(p => p.PanelMenuUserRelations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PanelMenuUserRelation_Users");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.ToTable("PaymentMode", "dbo");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Products");

            entity.ToTable("Product", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discountfixed).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsbnNumber)
                .HasMaxLength(200)
                .HasColumnName("isbn_number");
            entity.Property(e => e.MetaTitle).HasMaxLength(300);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.NetPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCompany).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(300);
            entity.Property(e => e.ProductTags).HasMaxLength(1000);
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("SKU");
            entity.Property(e => e.Url).HasMaxLength(300);
            entity.Property(e => e.Usprice)
                .IsUnicode(false)
                .HasColumnName("USPrice");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_Product_ProductBrands");
        });

        modelBuilder.Entity<ProductBrand>(entity =>
        {
            entity.ToTable("ProductBrands", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandName).HasMaxLength(150);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategories", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_ProductCategories_ProductCategories");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductCategories_Product");
        });

        modelBuilder.Entity<ProductPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId);

            entity.ToTable("ProductPhotos", "dbo");

            entity.Property(e => e.PhotoId).HasColumnName("PhotoID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPhotos)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductPhotos_Product");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("Publisher", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<RequestBook>(entity =>
        {
            entity.ToTable("RequestBook", "bookstore");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Author).HasMaxLength(50);
            entity.Property(e => e.BookName).HasMaxLength(50);
            entity.Property(e => e.Contact).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Service_Service");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.ToTable("Settings", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.FooterLogo).HasMaxLength(50);
            entity.Property(e => e.HomePageMetatDescription).HasMaxLength(500);
            entity.Property(e => e.HomePageMetatTitle).HasMaxLength(500);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Value).HasMaxLength(500);
            entity.Property(e => e.WebFavicon).HasMaxLength(50);
            entity.Property(e => e.WebLogo).HasMaxLength(50);
        });

        modelBuilder.Entity<ShippingProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ShippingProductDetail_ID");

            entity.ToTable("ShippingProductDetail", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductCode).IsUnicode(false);
            entity.Property(e => e.ProductName).IsUnicode(false);
            entity.Property(e => e.ProductPrice).IsUnicode(false);
            entity.Property(e => e.ProductVariation).IsUnicode(false);
            entity.Property(e => e.Productquantity).IsUnicode(false);
            entity.Property(e => e.ShippingOrderId).HasColumnName("ShippingOrderID");
            entity.Property(e => e.Skucode)
                .IsUnicode(false)
                .HasColumnName("SKUCode");

            entity.HasOne(d => d.ShippingOrder).WithMany(p => p.ShippingProductDetails)
                .HasForeignKey(d => d.ShippingOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShippingProductDetail_ShippingOrderID");
        });

        modelBuilder.Entity<SocialMedium>(entity =>
        {
            entity.ToTable("SocialMedia", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(500);
        });

        modelBuilder.Entity<StoreOwner>(entity =>
        {
            entity.ToTable("StoreOwners", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Prefix)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Prefixs).HasMaxLength(50);
            entity.Property(e => e.StoreName).HasMaxLength(50);
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.ToTable("Subscribers", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(500);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.ToTable("SystemSetting", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.FtpName).HasMaxLength(500);
            entity.Property(e => e.FtpPassword).HasMaxLength(500);
            entity.Property(e => e.Ftpip)
                .HasMaxLength(500)
                .HasColumnName("FTPIP");
            entity.Property(e => e.Ftppath)
                .HasMaxLength(500)
                .HasColumnName("FTPPath");
            entity.Property(e => e.IsSsl).HasColumnName("IsSSL");
            entity.Property(e => e.Port).HasMaxLength(50);
            entity.Property(e => e.SenderEmail).HasMaxLength(500);
            entity.Property(e => e.SenderPassword).HasMaxLength(500);
            entity.Property(e => e.Smtphost)
                .HasMaxLength(500)
                .HasColumnName("SMTPHost");
            entity.Property(e => e.SystemImage).HasMaxLength(50);
        });

        modelBuilder.Entity<TblBanner>(entity =>
        {
            entity.ToTable("tblBanners", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Banner).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnName("Created_Date");
            entity.Property(e => e.EndingDate).HasColumnName("Ending_Date");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.IsDelete).HasColumnName("Is_Delete");
            entity.Property(e => e.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Updated_Date");
            entity.Property(e => e.Url).HasMaxLength(200);
        });

        modelBuilder.Entity<TblCity>(entity =>
        {
            entity.ToTable("tblCity", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("cityName");
            entity.Property(e => e.CountryId).HasColumnName("countryId");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.IsDelete).HasColumnName("Is_Delete");
            entity.Property(e => e.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<TblColor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblColor__3214EC27858CC061");

            entity.ToTable("tblColor", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ColorName).HasMaxLength(50);
            entity.Property(e => e.ColorValue).HasMaxLength(150);
            entity.Property(e => e.CompanyColorValue).HasMaxLength(200);
        });

        modelBuilder.Entity<TblMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("tblMembers", "dbo");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Area).HasMaxLength(50);
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FacebookId)
                .HasMaxLength(50)
                .HasColumnName("facebookID");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.IsDelete).HasColumnName("Is_Delete");
            entity.Property(e => e.MemberName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("memberName");
            entity.Property(e => e.MemberTypeId).HasColumnName("memberTypeId");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("phoneNo");
            entity.Property(e => e.Photo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("photo");
            entity.Property(e => e.RecoverPassword).HasMaxLength(50);
            entity.Property(e => e.RegisterType).HasColumnName("registerType");
            entity.Property(e => e.TimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Updated_Date");

            entity.HasOne(d => d.City).WithMany(p => p.TblMembers)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_tblMembers_tblCity");
        });

        modelBuilder.Entity<TblPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblContentPages");

            entity.ToTable("tblPages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArticleContent)
                .HasDefaultValue("")
                .HasColumnName("Article_Content");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.Content2)
                .HasColumnType("text")
                .HasColumnName("content2");
            entity.Property(e => e.Content3)
                .HasColumnType("text")
                .HasColumnName("content3");
            entity.Property(e => e.Content4)
                .HasColumnType("text")
                .HasColumnName("content4");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.InsituteId).HasColumnName("insitute_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(1)
                .HasColumnName("Is_Active");
            entity.Property(e => e.MemberId).HasColumnName("Member_id");
            entity.Property(e => e.MetaDescription)
                .HasMaxLength(200)
                .HasColumnName("Meta_Description");
            entity.Property(e => e.MetaKeywords)
                .HasMaxLength(200)
                .HasColumnName("Meta_Keywords");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(100)
                .HasColumnName("Meta_Title");
            entity.Property(e => e.PanelId).HasColumnName("panel_id");
            entity.Property(e => e.PastpaperId).HasColumnName("pastpaper_id");
            entity.Property(e => e.QualificationId).HasColumnName("qualification_id");
            entity.Property(e => e.RelatedLink)
                .HasColumnType("text")
                .HasColumnName("related_link");
            entity.Property(e => e.SeoUrl)
                .HasMaxLength(250)
                .HasColumnName("SEO_URL");
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(2500)
                .HasColumnName("short_description");
            entity.Property(e => e.SubjId).HasColumnName("Subj_id");
            entity.Property(e => e.Thumbnail).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.YearId).HasColumnName("year_id");
        });

        modelBuilder.Entity<TblProductColor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblProdu__3214EC27E3289B3B");

            entity.ToTable("tblProductColors", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ColorId).HasColumnName("ColorID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Color).WithMany(p => p.TblProductColors)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK__tblProduc__Color__09A971A2");

            entity.HasOne(d => d.Product).WithMany(p => p.TblProductColors)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__tblProduc__Produ__6B24EA82");
        });

        modelBuilder.Entity<TblSlider>(entity =>
        {
            entity.ToTable("tblSliders", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.SubHeading)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.SubHeadingTwo)
                .HasMaxLength(600)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblTeamMember>(entity =>
        {
            entity.ToTable("tblTeamMembers", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Designation).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(500);
            entity.Property(e => e.Pinterest).HasColumnName("pinterest");
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
        });

        modelBuilder.Entity<TblVideo>(entity =>
        {
            entity.ToTable("tblVideo", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AltTag).HasMaxLength(200);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Dated).HasColumnType("datetime");
            entity.Property(e => e.EmbededCode).HasMaxLength(300);
            entity.Property(e => e.Image).HasMaxLength(200);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(200);
            entity.Property(e => e.VideoUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.ToTable("Testimonial", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Designation).HasMaxLength(250);
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<UrlbasedCmspage>(entity =>
        {
            entity.ToTable("URLBasedCMSPages", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Heading).HasMaxLength(500);
            entity.Property(e => e.IsGuide).HasMaxLength(50);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(500);
            entity.Property(e => e.PageName).HasMaxLength(500);
            entity.Property(e => e.Pageimage).HasMaxLength(500);
            entity.Property(e => e.Url).HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UrlbasedCmspages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_URLBasedCMSPages_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "dbo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ContactNo).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(500);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
