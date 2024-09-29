﻿// <auto-generated />
using System;
using AsyncJobsTemplate.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AsyncJobsTemplate.Infrastructure.Db.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AsyncJobsTemplate.Infrastructure.Db.Entities.JobEntity", b =>
                {
                    b.Property<long?>("RecId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("RecId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("RecId"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAtUtc");

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

                    b.Property<DateTime?>("LastUpdatedAtUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastUpdatedAtUtc");

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

                    b.HasKey("RecId");

                    b.ToTable("Job", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
