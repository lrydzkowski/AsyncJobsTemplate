using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Consumers;

internal class TopicConsumer<TOptions, TConsumer> : ServiceBusConsumer<TConsumer>
    where TOptions : class, ITopicOptions, new()
    where TConsumer : class, IMessageConsumer
{
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public TopicConsumer(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions,
        INamesResolver namesResolver,
        IServiceProvider serviceProvider,
        ILogger<TopicConsumer<TOptions, TConsumer>> logger
    ) : base(serviceBusClientFactory, serviceBusClientName, processorOptions, serviceProvider, logger)
    {
        _namesResolver = namesResolver;
        _options = options.Value;
    }

    protected override ServiceBusProcessor? CreateProcessor()
    {
        return _options.IsEnabled
            ? ServiceBusClient?.CreateProcessor(
                _namesResolver.ResolveTopicName(_options),
                _namesResolver.ResolveSubscriptionName(_options),
                ProcessorOptions
            )
            : null;
    }
}
