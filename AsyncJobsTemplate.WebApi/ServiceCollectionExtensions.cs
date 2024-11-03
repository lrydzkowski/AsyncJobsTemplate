using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.WebApi.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AsyncJobsTemplate.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagger();

        return services.AddMassTransit();
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AsyncJobTemplate", Version = "1.0" });
                options.EnableAnnotations();
            }
        );
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
                                e.MaxAutoRenewDuration = TimeSpan.FromHours(1);
                                e.ConfigureConsumer<JobsConsumer>(context);
                            }
                        );
                        cfg.ConfigureEndpoints(context);
                    }
                );
            }
        )!;
    }
}
