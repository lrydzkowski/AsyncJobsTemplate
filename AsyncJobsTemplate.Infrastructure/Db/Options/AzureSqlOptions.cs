namespace AsyncJobsTemplate.Infrastructure.Db.Options;

internal class AzureSqlOptions
{
    public const string Position = "AzureSql";

    public string ConnectionString { get; init; } = "";
}
