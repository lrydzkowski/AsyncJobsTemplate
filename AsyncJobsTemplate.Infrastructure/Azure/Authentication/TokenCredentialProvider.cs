﻿using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace AsyncJobsTemplate.Infrastructure.Azure.Authentication;

public static class TokenCredentialProvider
{
    private static TokenCredential? _tokenCredential;

    public static TokenCredential? Provide(IConfiguration configuration)
    {
        if (_tokenCredential is null)
        {
            List<TokenCredential> tokenCredentials = [];
            AddClientSecretCredential(tokenCredentials, configuration);
            if (tokenCredentials.Count == 0)
            {
                return null;
            }

            _tokenCredential = new ChainedTokenCredential(tokenCredentials.ToArray());
        }

        return _tokenCredential;
    }

    private static void AddClientSecretCredential(List<TokenCredential> tokenCredentials, IConfiguration configuration)
    {
        string? tenantId = configuration.GetValue(nameof(AzureAdOptions.TenantId));
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return;
        }

        string? clientId = configuration.GetValue(nameof(AzureAdOptions.ClientId));
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return;
        }

        string? clientSecret = configuration.GetValue(nameof(AzureAdOptions.ClientSecret));
        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            return;
        }

        tokenCredentials.Add(
            new ClientSecretCredential(tenantId, clientId, clientSecret)
        );
    }

    private static string? GetValue(this IConfiguration configuration, string key)
    {
        return configuration[$"{AzureAdOptions.Position}:{key}"];
    }
}
