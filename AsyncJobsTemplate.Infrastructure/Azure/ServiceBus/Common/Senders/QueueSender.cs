using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Senders;

internal class QueueSender<TData, TOptions> : ServiceBusSenderBase<TData>
    where TData : class where TOptions : class, IQueueOptions, new()
{
    public QueueSender(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName,
        INamesResolver namesResolver
    ) : base(serviceBusClientFactory, messageSerializer, serviceBusClientName)
    {
        ServiceBusSender = ServiceBusClient.CreateSender(namesResolver.ResolveQueueName(options.Value));
    }
}
