using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.WebApi.Consumers;
using Azure.Core;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AsyncJobsTemplate.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagger();

        return services.AddMassTransit(configuration);
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

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
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
                        cfg.Host(
                            options.HostAddress,
                            configurator =>
                            {
                                TokenCredential? tokenCredential = TokenCredentialProvider.Provide(configuration);
                                if (tokenCredential is null)
                                {
                                    throw new InvalidOperationException(
                                        "TokenCredential is not available for generating an access token"
                                    );
                                }

                                configurator.TokenCredential = tokenCredential;
                            }
                        );
                        cfg.ReceiveEndpoint(
                            options.JobQueueName,
                            configurator =>
                            {
                                configurator.MaxAutoRenewDuration = TimeSpan.FromHours(1);
                                configurator.ConfigureConsumer<JobsConsumer>(context);
                            }
                        );
                        cfg.ConfigureEndpoints(context);
                    }
                );
            }
        )!;
    }
}
