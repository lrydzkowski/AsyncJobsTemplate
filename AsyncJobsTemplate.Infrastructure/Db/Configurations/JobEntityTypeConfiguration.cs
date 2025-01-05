using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsyncJobsTemplate.Infrastructure.Db.Configurations;

internal class JobEntityTypeConfiguration : IEntityTypeConfiguration<JobEntity>
{
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        SetTableName(builder);
        SetPrimaryKey(builder);
        ConfigureColumns(builder);
    }

    private void SetTableName(EntityTypeBuilder<JobEntity> builder)
    {
        builder.ToTable(JobEntity.TableName);
    }

    private void SetPrimaryKey(EntityTypeBuilder<JobEntity> builder)
    {
        builder.HasKey(company => company.RecId);
    }

    private void ConfigureColumns(EntityTypeBuilder<JobEntity> builder)
    {
        builder.Property(company => company.RecId)
            .HasColumnName(nameof(JobEntity.RecId))
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(company => company.UserEmail)
            .HasColumnName(nameof(JobEntity.UserEmail))
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(company => company.JobId)
            .HasColumnName(nameof(JobEntity.JobId))
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(company => company.JobCategoryName)
            .HasColumnName(nameof(JobEntity.JobCategoryName))
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(company => company.Status)
            .HasColumnName(nameof(JobEntity.Status))
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(company => company.InputData)
            .HasColumnName(nameof(JobEntity.InputData))
            .HasMaxLength(10000);
        builder.Property(company => company.InputFileReference)
            .HasColumnName(nameof(JobEntity.InputFileReference))
            .HasMaxLength(200);
        builder.Property(company => company.OutputData)
            .HasColumnName(nameof(JobEntity.OutputData))
            .HasMaxLength(10000);
        builder.Property(company => company.OutputFileReference)
            .HasColumnName(nameof(JobEntity.OutputFileReference))
            .HasMaxLength(200);
        builder.Property(company => company.Errors)
            .HasColumnName(nameof(JobEntity.Errors))
            .HasMaxLength(10000);
        builder.Property(company => company.CreatedAt)
            .HasColumnName(nameof(JobEntity.CreatedAt))
            .IsRequired();
        builder.Property(company => company.LastUpdatedAt)
            .HasColumnName(nameof(JobEntity.LastUpdatedAt));
    }
}
