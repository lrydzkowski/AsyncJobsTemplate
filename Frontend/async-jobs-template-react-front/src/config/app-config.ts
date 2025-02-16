class AppConfig {
  azureAdClientId = this.getEnvironmentValue('VITE_AZURE_AD_CLIENT_ID');
  azureAdScopes = this.getEnvironmentValue('VITE_AZURE_AD_SCOPES').split(' ') ?? [];
  azureAdTenantId = this.getEnvironmentValue('VITE_AZURE_AD_TENANT_ID');
  azureAdRedirectUri = this.getEnvironmentValue('VITE_AZURE_AD_REDIRECT_URI');

  apiBaseUrl = this.getEnvironmentValue('VITE_API_BASE_URL');

  mode = this.getEnvironmentValue('MODE');

  private getEnvironmentValue(key: string): string {
    const value: unknown = import.meta.env[key];
    if (!value) {
      console.warn(`Environment variable ${key} is not defined`);
    }

    return (value || '') as string;
  }
}

const appConfig = new AppConfig();
export default appConfig;
