﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProtoLib.Model;

#nullable disable

namespace ProtoLib.Migrations
{
    [DbContext(typeof(BaseContext))]
    partial class BaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ProtoLib.Model.ImageSet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ImageSet");
                });

            modelBuilder.Entity("ProtoLib.Model.Post", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<bool>("Disabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsShared")
                        .HasColumnType("bit");

                    b.Property<int>("ProductOrder")
                        .HasColumnType("int");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Name");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("ProtoLib.Model.PostCreationKey", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("PostKeys");
                });

            modelBuilder.Entity("ProtoLib.Model.PostStatistic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int>("ActualEvents")
                        .HasColumnType("int");

                    b.Property<decimal>("IncomeCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PredictCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("RunningCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SendedCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Stamp")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("WaitingCost")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("PostStatistics");
                });

            modelBuilder.Entity("ProtoLib.Model.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("MasterPostMap")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserAccName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("UserAccName");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ProtoLib.Model.StoredImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ImageSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InitialFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ImageSetId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<Guid?>("ImageSetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ImageSetId");

                    b.ToTable("TechCards");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCardLine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Equipment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EquipmentInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ImageSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsCustom")
                        .HasColumnType("bit");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProcessionOrder")
                        .HasColumnType("int");

                    b.Property<Guid>("TechCardPostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TechCardPostId");

                    b.ToTable("TechCardLines");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCardPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ImageSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<Guid>("TechCardId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ImageSetId");

                    b.HasIndex("PostId");

                    b.HasIndex("TechCardId");

                    b.ToTable("TechCardPosts");
                });

            modelBuilder.Entity("ProtoLib.Model.Transfer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("datetime2");

                    b.Property<string>("ClosedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ClosedStamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaperId")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("PostFromId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("PostToId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("PostFromId");

                    b.HasIndex("PostToId");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("ProtoLib.Model.TransferLine", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<bool>("IsTransfered")
                        .HasColumnType("bit");

                    b.Property<int>("OrderLineNumber")
                        .HasColumnType("int");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("ProductionLine")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SourceWorkCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("SourceWorkId")
                        .HasColumnType("bigint");

                    b.Property<long?>("TargetWorkId")
                        .HasColumnType("bigint");

                    b.Property<long?>("TransferId")
                        .HasColumnType("bigint");

                    b.Property<int>("TransferedCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TransferId");

                    b.ToTable("TransferLines");
                });

            modelBuilder.Entity("ProtoLib.Model.User", b =>
                {
                    b.Property<string>("AccName")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("AccName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProtoLib.Model.Work", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("CommentMap")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedStamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DeadLine")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("MovedFrom")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("MovedTo")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("OrderLineNumber")
                        .HasColumnType("int");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ProductLine")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<decimal>("SingleCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Works");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkIssue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Resolved")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReturnBackPostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReturnedFromPostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TemplateId")
                        .HasColumnType("bigint");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.HasIndex("WorkId");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkIssueLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("End")
                        .HasColumnType("datetime2");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ReturnedToPost")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SourceIssueId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("WorkIssueLogs");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkIssueTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IssueTemplates");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkPriority", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("DateChange")
                        .HasColumnType("datetime2");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("WorkPriorities");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkStatusLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Article")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("EditedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NewStatus")
                        .HasColumnType("int");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("PrevStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("Stamp")
                        .HasColumnType("datetime2");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("WorkStatusLogs");
                });

            modelBuilder.Entity("ProtoLib.Model.PostCreationKey", b =>
                {
                    b.HasOne("ProtoLib.Model.Post", null)
                        .WithMany("PostCreationKeys")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProtoLib.Model.Role", b =>
                {
                    b.HasOne("ProtoLib.Model.User", null)
                        .WithMany("Roles")
                        .HasForeignKey("UserAccName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProtoLib.Model.StoredImage", b =>
                {
                    b.HasOne("ProtoLib.Model.ImageSet", null)
                        .WithMany("Images")
                        .HasForeignKey("ImageSetId");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCard", b =>
                {
                    b.HasOne("ProtoLib.Model.ImageSet", "ImageSet")
                        .WithMany()
                        .HasForeignKey("ImageSetId");

                    b.Navigation("ImageSet");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCardLine", b =>
                {
                    b.HasOne("ProtoLib.Model.TechCardPost", null)
                        .WithMany("Lines")
                        .HasForeignKey("TechCardPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProtoLib.Model.TechCardPost", b =>
                {
                    b.HasOne("ProtoLib.Model.ImageSet", "ImageSet")
                        .WithMany()
                        .HasForeignKey("ImageSetId");

                    b.HasOne("ProtoLib.Model.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProtoLib.Model.TechCard", null)
                        .WithMany("PostParts")
                        .HasForeignKey("TechCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ImageSet");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("ProtoLib.Model.Transfer", b =>
                {
                    b.HasOne("ProtoLib.Model.Post", "PostFrom")
                        .WithMany()
                        .HasForeignKey("PostFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProtoLib.Model.Post", "PostTo")
                        .WithMany()
                        .HasForeignKey("PostToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PostFrom");

                    b.Navigation("PostTo");
                });

            modelBuilder.Entity("ProtoLib.Model.TransferLine", b =>
                {
                    b.HasOne("ProtoLib.Model.Transfer", null)
                        .WithMany("Lines")
                        .HasForeignKey("TransferId");
                });

            modelBuilder.Entity("ProtoLib.Model.Work", b =>
                {
                    b.HasOne("ProtoLib.Model.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkIssue", b =>
                {
                    b.HasOne("ProtoLib.Model.WorkIssueTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProtoLib.Model.Work", "Work")
                        .WithMany("Issues")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Template");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("ProtoLib.Model.ImageSet", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("ProtoLib.Model.Post", b =>
                {
                    b.Navigation("PostCreationKeys");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCard", b =>
                {
                    b.Navigation("PostParts");
                });

            modelBuilder.Entity("ProtoLib.Model.TechCardPost", b =>
                {
                    b.Navigation("Lines");
                });

            modelBuilder.Entity("ProtoLib.Model.Transfer", b =>
                {
                    b.Navigation("Lines");
                });

            modelBuilder.Entity("ProtoLib.Model.User", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("ProtoLib.Model.Work", b =>
                {
                    b.Navigation("Issues");
                });
#pragma warning restore 612, 618
        }
    }
}
