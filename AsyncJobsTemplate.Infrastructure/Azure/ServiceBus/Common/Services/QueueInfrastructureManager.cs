using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using Azure;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;

internal class QueueInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, IQueueOptions
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public QueueInfrastructureManager(
        IOptions<TOptions> options,
        INamesResolver namesResolver,
        ILogger<QueueInfrastructureManager<TOptions>> logger,
        IConfiguration configuration
    )
    {
        _options = options.Value;
        _namesResolver = namesResolver;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateQueueAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Service Bus infrastructure");
        }
    }

    private async Task CreateQueueAsync(IQueueOptions queueOptions, CancellationToken cancellationToken)
    {
        if (!queueOptions.CreateQueueOnStartup)
        {
            return;
        }

        string queueName = _namesResolver.ResolveQueueName(queueOptions);

        _logger.LogInformation("Creating queue: {QueueName}", queueName);

        try
        {
            ServiceBusAdministrationClient client = BuildServiceBusAdministrationClient(queueOptions);
            Response<bool>? queueExists = await client.QueueExistsAsync(queueName, cancellationToken);
            if (queueExists?.Value == true)
            {
                _logger.LogInformation("Queue already exists: {QueueName}", queueName);

                return;
            }

            await client.CreateQueueAsync(queueName, cancellationToken);
            _logger.LogInformation("Queue created: {QueueName}", queueName);
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation("Queue already exists: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create queue: {QueueName}", queueName);
        }
    }

    private ServiceBusAdministrationClient BuildServiceBusAdministrationClient(IQueueOptions queueOptions)
    {
        if (string.IsNullOrWhiteSpace(queueOptions.HostAddress))
        {
            return new ServiceBusAdministrationClient(queueOptions.ConnectionString);
        }

        TokenCredential? tokenCredential = TokenCredentialProvider.Provide(_configuration);
        if (tokenCredential is null)
        {
            throw new InvalidOperationException("TokenCredential is not provided");
        }

        return new ServiceBusAdministrationClient(queueOptions.HostAddress, tokenCredential);
    }
}
