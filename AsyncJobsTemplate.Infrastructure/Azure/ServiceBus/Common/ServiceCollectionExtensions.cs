using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Consumers;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Senders;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Services;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusCommonServices(this IServiceCollection services)
    {
        return services.AddServices();
    }

    public static IServiceCollection AddServiceBusQueueConsumer<TConsumer, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    ) where TConsumer : class, IMessageConsumer where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TConsumer).Name;
        services.AddServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<QueueConsumer<TOptions, TConsumer>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddScoped<TConsumer>();

        return services;
    }

    public static IServiceCollection AddServiceBusTopicConsumer<TConsumer, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    ) where TConsumer : class, IMessageConsumer where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TConsumer).Name;
        services.AddServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<TopicConsumer<TOptions, TConsumer>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddScoped<TConsumer>();

        return services;
    }

    public static IServiceCollection AddServiceBusQueueSender<TSender, TSenderImplementation, TData, TOptions>(
        this IServiceCollection services
    )
        where TSender : class
        where TSenderImplementation : class, TSender
        where TData : class
        where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TSender).Name;
        services.AddServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusSender<TData>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<QueueSender<TData, TOptions>>(serviceProvider, name)
        );
        services.AddScoped<TSender, TSenderImplementation>();

        return services;
    }

    public static IServiceCollection AddServiceBusTopicSender<TSender, TSenderImplementation, TData, TOptions>(
        this IServiceCollection services
    )
        where TSender : class
        where TSenderImplementation : class, TSender
        where TData : class
        where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TSender).Name;
        services.AddServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusSender<TData>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<TopicSender<TData, TOptions>>(serviceProvider, name)
        );
        services.AddScoped<TSender, TSenderImplementation>();

        return services;
    }

    public static IServiceCollection AddQueueCreator<TOptions>(this IServiceCollection services)
        where TOptions : class, IQueueOptions
    {
        services.AddSingleton<IInfrastructureManager, QueueInfrastructureManager<TOptions>>();

        return services;
    }

    public static IServiceCollection AddTopicCreator<TOptions>(this IServiceCollection services)
        where TOptions : class, ITopicOptions
    {
        services.AddSingleton<IInfrastructureManager, TopicInfrastructureManager<TOptions>>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<INamesResolver, NamesResolver>();
        services.AddHostedService<WorkerServiceBus>();

        return services;
    }

    private static IServiceCollection AddServiceBusClient<TOptions>(this IServiceCollection services, string name)
        where TOptions : class, IServiceBusOptions, new()
    {
        services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddClient<ServiceBusClient, ServiceBusClientOptions>((_, serviceProvider) =>
                        {
                            TOptions options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;
                            if (!options.IsEnabled)
                            {
                                return null!;
                            }

                            if (string.IsNullOrWhiteSpace(options.HostAddress))
                            {
                                return new ServiceBusClient(options.ConnectionString);
                            }

                            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                            TokenCredential? tokenCredential = TokenCredentialProvider.Provide(configuration);
                            if (tokenCredential is null)
                            {
                                throw new InvalidOperationException("TokenCredential is not provided");
                            }

                            return new ServiceBusClient(options.HostAddress, tokenCredential);
                        }
                    )
                    .WithName(name);
            }
        );

        return services;
    }
}
