namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;

public interface IInfrastructureManager
{
    Task CreateInfrastructureAsync(CancellationToken cancellationToken = default);
}
