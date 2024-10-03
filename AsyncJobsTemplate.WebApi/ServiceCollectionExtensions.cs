using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.WebApi.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        return services.AddMassTransit();
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services)
    {
        return services.AddMassTransit(
            x =>
            {
                x.AddConsumer<JobsConsumer>();
                x.UsingAzureServiceBus(
                    (context, cfg) =>
                    {
                        AzureServiceBusOptions options =
                            context.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value;
                        cfg.Host(options.ConnectionString);
                        cfg.ReceiveEndpoint(
                            options.JobQueueName,
                            e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.PublishFaults = false;
                                e.MaxAutoRenewDuration = TimeSpan.FromHours(1);
                                e.ConfigureConsumer<JobsConsumer>(context);
                            }
                        );
                        cfg.ConfigureEndpoints(context);
                    }
                );
            }
        );
    }
}
