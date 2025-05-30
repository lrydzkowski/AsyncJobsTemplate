using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;

internal class TopicInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, ITopicOptions
{
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public TopicInfrastructureManager(
        IAzureClientFactory<ServiceBusAdministrationClient> clientFactory,
        string serviceBusClientName,
        IOptions<TOptions> options,
        INamesResolver namesResolver,
        ILogger<TopicInfrastructureManager<TOptions>> logger
    )
    {
        _adminClient = clientFactory.CreateClient(serviceBusClientName);
        _options = options.Value;
        _namesResolver = namesResolver;
        _logger = logger;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateTopicAsync(_options, cancellationToken);
            await CreateSubscriptionAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Service Bus infrastructure");
        }
    }

    private async Task CreateTopicAsync(
        ITopicOptions topicOptions,
        CancellationToken cancellationToken
    )
    {
        if (!topicOptions.IsEnabled || !topicOptions.CreateTopicOnStartup)
        {
            return;
        }

        string topicName = _namesResolver.ResolveTopicName(topicOptions);

        _logger.LogInformation("Creating topic: {TopicName}", topicName);

        try
        {
            Response<bool>? topicExists = await _adminClient.TopicExistsAsync(topicName, cancellationToken);
            if (topicExists?.Value == true)
            {
                _logger.LogInformation("Topic already exists: {TopicName}", topicName);

                return;
            }

            await _adminClient.CreateTopicAsync(topicName, cancellationToken);
            _logger.LogInformation("Topic created: {TopicName}", topicName);
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation("Topic already exists: {TopicName}", topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create topic: {TopicName}", topicName);
        }
    }

    private async Task CreateSubscriptionAsync(ITopicOptions topicOptions, CancellationToken cancellationToken)
    {
        if (!topicOptions.IsEnabled || !topicOptions.CreateSubscriptionOnStartup)
        {
            return;
        }

        string topicName = _namesResolver.ResolveTopicName(topicOptions);
        string subscriptionName = _namesResolver.ResolveSubscriptionName(topicOptions);

        _logger.LogInformation(
            "Creating subscription: {SubscriptionName} for topic: {TopicName}",
            subscriptionName,
            topicName
        );

        try
        {
            Response<bool>? subscriptionExists = await _adminClient.SubscriptionExistsAsync(
                topicName,
                subscriptionName,
                cancellationToken
            );
            if (subscriptionExists?.Value == true)
            {
                _logger.LogInformation(
                    "Subscription already exists: {SubscriptionName} for topic: {TopicName}",
                    subscriptionName,
                    topicName
                );

                return;
            }

            await _adminClient.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
            _logger.LogInformation(
                "Subscription created: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation(
                "Subscription already exists: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create subscription: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
    }
}
