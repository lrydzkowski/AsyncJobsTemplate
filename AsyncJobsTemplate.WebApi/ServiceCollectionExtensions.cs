using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common;
using AsyncJobsTemplate.WebApi.Authentication;
using AsyncJobsTemplate.WebApi.Consumers;
using AsyncJobsTemplate.WebApi.Mappers;
using AsyncJobsTemplate.WebApi.Options;
using AsyncJobsTemplate.WebApi.Services;
using AsyncJobsTemplate.WebApi.Swagger;
using Azure.Messaging.ServiceBus;
using Microsoft.OpenApi.Models;

namespace AsyncJobsTemplate.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.AddOptions(configuration);
        services.AddServices();
        services.AddCorsDefaultPolicy(configuration);
        services.AddApiKeyAuthentication();
        services.AddServiceBusConsumers();

        return services;
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
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
                options.OperationFilter<SwaggerHeaderParameterAttributeFilter>();
            }
        );
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<SwaggerOptions>(configuration, SwaggerOptions.Position)
            .AddOptionsType<InternalEndpointsOptions>(configuration, InternalEndpointsOptions.Position);
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
        return services.AddCors(options =>
            {
                string[] allowedOrigins = configuration["AllowedOrigins"]?.Split(";") ?? [];

                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );
            }
        );
    }

    private static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationSchemeOptions.Name,
                null
            );

        return services;
    }

    private static IServiceCollection AddServiceBusConsumers(this IServiceCollection services)
    {
        services.AddServiceBusQueueConsumer<JobsQueueConsumer, AzureServiceBusOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromHours(1),
                MaxConcurrentCalls = 1
            }
        );

        return services;
    }
}
