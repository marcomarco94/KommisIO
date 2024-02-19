﻿// <auto-generated />
using System;
using DataRESTfulAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataRESTfulAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240212220713_dataModelConversion")]
    partial class dataModelConversion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BackendDataAccessLayer.Entity.ArticleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Height")
                        .HasColumnType("real");

                    b.Property<float>("Length")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<float>("Width")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId")
                        .IsUnique();

                    b.ToTable("Article");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.DamageReportEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("DamageReportEntities");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.EmployeeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<short>("PersonnelNumber")
                        .HasColumnType("smallint");

                    b.Property<byte>("Role")
                        .HasColumnType("tinyint");

                    b.Property<string>("_passwordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PersonnelNumber")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.PickingOrderEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("Priority")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("PickingOrders");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.PickingOrderPositionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("DesiredAmount")
                        .HasColumnType("int");

                    b.Property<int>("PickedAmount")
                        .HasColumnType("int");

                    b.Property<int?>("PickingOrderEntityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("PickingOrderEntityId");

                    b.ToTable("PickingOrderPositions");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.StockPositionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("ShelfNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.ToTable("StockPositions");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.DamageReportEntity", b =>
                {
                    b.HasOne("BackendDataAccessLayer.Entity.ArticleEntity", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");

                    b.HasOne("BackendDataAccessLayer.Entity.EmployeeEntity", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.Navigation("Article");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.PickingOrderEntity", b =>
                {
                    b.HasOne("BackendDataAccessLayer.Entity.EmployeeEntity", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.PickingOrderPositionEntity", b =>
                {
                    b.HasOne("BackendDataAccessLayer.Entity.ArticleEntity", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");

                    b.HasOne("BackendDataAccessLayer.Entity.PickingOrderEntity", null)
                        .WithMany("Positions")
                        .HasForeignKey("PickingOrderEntityId");

                    b.Navigation("Article");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.StockPositionEntity", b =>
                {
                    b.HasOne("BackendDataAccessLayer.Entity.ArticleEntity", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");

                    b.Navigation("Article");
                });

            modelBuilder.Entity("BackendDataAccessLayer.Entity.PickingOrderEntity", b =>
                {
                    b.Navigation("Positions");
                });
#pragma warning restore 612, 618
        }
    }
}
