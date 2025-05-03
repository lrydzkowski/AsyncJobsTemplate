using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Senders;

internal class TopicSender<TData, TOptions> : ServiceBusSenderBase<TData>
    where TData : class where TOptions : class, ITopicOptions, new()
{
    public TopicSender(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName,
        INamesResolver namesResolver
    ) : base(serviceBusClientFactory, messageSerializer, serviceBusClientName)
    {
        ServiceBusSender = ServiceBusClient.CreateSender(namesResolver.ResolveTopicName(options.Value));
    }
}
