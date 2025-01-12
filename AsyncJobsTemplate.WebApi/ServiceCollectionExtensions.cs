using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.WebApi.Consumers;
using AsyncJobsTemplate.WebApi.Mappers;
using AsyncJobsTemplate.WebApi.Options;
using AsyncJobsTemplate.WebApi.Services;
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
        services.AddMassTransit(configuration);
        services.AddOptions(configuration);
        services.AddServices();
        services.AddCorsDefaultPolicy(configuration);

        return services;
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AsyncJobTemplate", Version = "1.0" });
                options.EnableAnnotations();
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer"
                    }
                );
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            []
                        }
                    }
                );
            }
        );
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMassTransit(
            x =>
            {
                x.AddConsumer<JobsConsumer>();
                if (QueueOptions.IsQueueInMemory(configuration))
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

                    return;
                }

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

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<SwaggerOptions>(configuration, SwaggerOptions.Position)
            .AddOptionsType<QueueOptions>(configuration, QueueOptions.Position);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<ITriggerJobResponseMapper, TriggerJobResponseMapper>()
            .AddScoped<IUserEmailProvider, UserEmailProvider>()
            .AddScoped<IRequestContextProvider, RequestContextProvider>()
            .AddScoped<ICheckHealthResponseMapper, CheckHealthResponseMapper>();
    }

    private static IServiceCollection AddCorsDefaultPolicy(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddCors(
            options =>
            {
                string[] allowedOrigins = configuration["AllowedOrigins"]?.Split(";") ?? [];

                options.AddDefaultPolicy(
                    builder => builder.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );
            }
        );
    }
}
