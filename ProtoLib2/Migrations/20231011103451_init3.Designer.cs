﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProtoLib2.Model;

#nullable disable

namespace ProtoLib2.Migrations
{
    [DbContext(typeof(BaseContext))]
    [Migration("20231011103451_init3")]
    partial class init3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ProtoLib.Model.Post", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

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

            modelBuilder.Entity("ProtoLib.Model.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("PostName")
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserAccName")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("PostName");

                    b.HasIndex("UserAccName");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ProtoLib.Model.User", b =>
                {
                    b.Property<string>("AccName")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

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

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<long>("OrderNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<float>("SingleCost")
                        .HasColumnType("real");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Works");
                });

            modelBuilder.Entity("ProtoLib.Model.WorkLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("EditedByAccName")
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("NewStatus")
                        .HasColumnType("int");

                    b.Property<int>("PrevStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("Stamp")
                        .HasColumnType("datetime2");

                    b.Property<long>("WorkId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EditedByAccName");

                    b.HasIndex("WorkId");

                    b.ToTable("WorkLogs");
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
                    b.HasOne("ProtoLib.Model.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostName");

                    b.HasOne("ProtoLib.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserAccName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
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

            modelBuilder.Entity("ProtoLib.Model.WorkLog", b =>
                {
                    b.HasOne("ProtoLib.Model.User", "EditedBy")
                        .WithMany()
                        .HasForeignKey("EditedByAccName");

                    b.HasOne("ProtoLib.Model.Work", "Work")
                        .WithMany()
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EditedBy");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("ProtoLib.Model.Post", b =>
                {
                    b.Navigation("PostCreationKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
