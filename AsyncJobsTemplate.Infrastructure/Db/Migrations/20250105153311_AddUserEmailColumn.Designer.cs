﻿// <auto-generated />
using System;
using AsyncJobsTemplate.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AsyncJobsTemplate.Infrastructure.Db.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250105153311_AddUserEmailColumn")]
    partial class AddUserEmailColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AsyncJobsTemplate.Infrastructure.Db.Entities.JobEntity", b =>
                {
                    b.Property<long>("RecId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("RecId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RecId"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("Errors")
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Errors");

                    b.Property<string>("InputData")
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("InputData");

                    b.Property<string>("InputFileReference")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("InputFileReference");

                    b.Property<string>("JobCategoryName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("JobCategoryName");

                    b.Property<Guid>("JobId")
                        .HasMaxLength(200)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("JobId");

                    b.Property<DateTimeOffset?>("LastUpdatedAt")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("LastUpdatedAt");

                    b.Property<string>("OutputData")
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("OutputData");

                    b.Property<string>("OutputFileReference")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("OutputFileReference");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("Status");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("UserEmail");

                    b.HasKey("RecId");

                    b.ToTable("Job", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
