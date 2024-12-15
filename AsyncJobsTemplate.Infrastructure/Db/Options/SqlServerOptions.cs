namespace AsyncJobsTemplate.Infrastructure.Db.Options;

internal class SqlServerOptions
{
    public const string Position = "SqlServer";

    public string ConnectionString { get; init; } = "";
}
