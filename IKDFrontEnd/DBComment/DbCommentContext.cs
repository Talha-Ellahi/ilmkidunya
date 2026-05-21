using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.DBComment;

public partial class DbCommentContext : DbContext
{
    public DbCommentContext()
    {
    }

    public DbCommentContext(DbContextOptions<DbCommentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblComments2> TblComments2s { get; set; }

    public virtual DbSet<TblCommentsChild> TblCommentsChildren { get; set; }

    public virtual DbSet<TblDefComment> TblDefComments { get; set; }

    public virtual DbSet<TblDefCommentLike> TblDefCommentLikes { get; set; }

    public virtual DbSet<TblDefMemberInfo2> TblDefMemberInfo2s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=66.23.236.70;Initial Catalog=dbcomments;User Id=userdbcomments;Password=?LdfKLn22!fxzy5b;TrustServerCertificate=True;Connection Timeout=300;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("userdbcomments");

        modelBuilder.Entity<TblComments2>(entity =>
        {
            entity.HasKey(e => e.CommentId);

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
            entity.HasKey(e => e.ChildCmtId);

            entity.ToTable("tblComments_Child", "dbo");

            entity.Property(e => e.ChildCmtId).HasColumnName("ChildCmt_Id");
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

        modelBuilder.Entity<TblDefComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("TblDefComments", "dbikduser");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PageTitle).HasMaxLength(500);
            entity.Property(e => e.PageUrl).HasMaxLength(500);
            entity.Property(e => e.PageUrlNoSlash).HasMaxLength(4000);
            entity.Property(e => e.UserEmail).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.UserProfilePicture).HasMaxLength(500);
        });

        modelBuilder.Entity<TblDefCommentLike>(entity =>
        {
            entity.HasKey(e => e.LikeId);

            entity.ToTable("TblDefCommentLikes", "dbikduser");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Comment).WithMany(p => p.TblDefCommentLikes)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentLikes_Comments");
        });

        modelBuilder.Entity<TblDefMemberInfo2>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("TblDefMemberInfo2", "dbo");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
