﻿// <auto-generated />
using System;
using Box.CMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Box.CMS.Migrations
{
    [DbContext(typeof(CMSContext))]
    partial class CMSContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Box.CMS.Models.ContentComment", b =>
                {
                    b.Property<string>("CommentUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Author")
                        .HasMaxLength(50);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("CommentDate");

                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("ParentCommentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<int>("Position");

                    b.Property<short>("StartRank");

                    b.Property<short>("Status");

                    b.HasKey("CommentUId");

                    b.ToTable("ContentComments");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentCommentCount", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<int>("Count");

                    b.HasKey("ContentUId");

                    b.ToTable("ContentCommentCounts");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentCustomInfo", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<DateTime?>("Date1");

                    b.Property<DateTime?>("Date2");

                    b.Property<DateTime?>("Date3");

                    b.Property<DateTime?>("Date4");

                    b.Property<double?>("Number1");

                    b.Property<double?>("Number2");

                    b.Property<double?>("Number3");

                    b.Property<double?>("Number4");

                    b.Property<string>("Text1")
                        .HasMaxLength(2000);

                    b.Property<string>("Text2")
                        .HasMaxLength(2000);

                    b.Property<string>("Text3")
                        .HasMaxLength(2000);

                    b.Property<string>("Text4")
                        .HasMaxLength(2000);

                    b.HasKey("ContentUId");

                    b.ToTable("ContentCustomInfos");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentData", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasMaxLength(36);

                    b.Property<string>("JSON");

                    b.HasKey("ContentUId");

                    b.ToTable("ContentDatas");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentHead", b =>
                {
                    b.Property<string>("ContentUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Abstract")
                        .HasMaxLength(300);

                    b.Property<string>("CanonicalName")
                        .HasMaxLength(250);

                    b.Property<DateTime>("ContentDate");

                    b.Property<DateTime>("CreateDate");

                    b.Property<short>("DisplayOrder");

                    b.Property<string>("ExternalLinkUrl")
                        .HasMaxLength(500);

                    b.Property<string>("Kind")
                        .HasMaxLength(25);

                    b.Property<string>("Location")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("PublishAfter");

                    b.Property<DateTime?>("PublishUntil");

                    b.Property<string>("ThumbFilePath")
                        .HasMaxLength(87);

                    b.Property<int>("Version");

                    b.HasKey("ContentUId");

                    b.ToTable("ContentHeads");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentPageViewCount", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<int>("Count");

                    b.HasKey("ContentUId");

                    b.ToTable("ContentPageViewCounts");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentShareCount", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<long>("Count");

                    b.HasKey("ContentUId");

                    b.ToTable("ContentSharesCounts");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentTag", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Tag")
                        .HasMaxLength(100);

                    b.HasKey("ContentUId", "Tag");

                    b.ToTable("ContentTags");
                });

            modelBuilder.Entity("Box.CMS.Models.CrossLink", b =>
                {
                    b.Property<string>("ContentUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("PageArea")
                        .HasMaxLength(50);

                    b.Property<short>("DisplayOrder");

                    b.HasKey("ContentUId", "PageArea");

                    b.ToTable("CrossLinks");
                });

            modelBuilder.Entity("Box.CMS.Models.File", b =>
                {
                    b.Property<string>("FileUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("FileName")
                        .HasMaxLength(255);

                    b.Property<string>("Folder")
                        .HasMaxLength(50);

                    b.Property<int>("Size");

                    b.Property<string>("Type")
                        .HasMaxLength(100);

                    b.Property<DateTime>("_CreateDate");

                    b.HasKey("FileUId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Box.CMS.Models.FileData", b =>
                {
                    b.Property<string>("FileUId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<byte[]>("StoredData");

                    b.Property<byte[]>("StoredThumbData");

                    b.HasKey("FileUId");

                    b.ToTable("FileData");
                });

            modelBuilder.Entity("Box.CMS.Models.ContentCommentCount", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithOne("CommentsCount")
                        .HasForeignKey("Box.CMS.Models.ContentCommentCount", "ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.ContentCustomInfo", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithOne("CustomInfo")
                        .HasForeignKey("Box.CMS.Models.ContentCustomInfo", "ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.ContentData", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithOne("Data")
                        .HasForeignKey("Box.CMS.Models.ContentData", "ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.ContentPageViewCount", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithOne("PageViewCount")
                        .HasForeignKey("Box.CMS.Models.ContentPageViewCount", "ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.ContentShareCount", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithOne("ShareCount")
                        .HasForeignKey("Box.CMS.Models.ContentShareCount", "ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.ContentTag", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithMany("Tags")
                        .HasForeignKey("ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.CrossLink", b =>
                {
                    b.HasOne("Box.CMS.Models.ContentHead")
                        .WithMany("CrossLinks")
                        .HasForeignKey("ContentUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Box.CMS.Models.FileData", b =>
                {
                    b.HasOne("Box.CMS.Models.File")
                        .WithOne("Data")
                        .HasForeignKey("Box.CMS.Models.FileData", "FileUId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}