import { LogLevel } from '@azure/msal-browser';
import appConfig from '../config/app-config';

export const msalConfig = {
  auth: {
    clientId: appConfig.azureAdClientId,
    authority: `https://login.microsoftonline.com/${appConfig.azureAdTenantId}`,
    redirectUri: appConfig.azureAdRedirectUri,
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: unknown, message: unknown, containsPii: unknown) => {
        if (containsPii || appConfig.mode === 'production') {
          return;
        }

        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
    },
  },
};

export const loginRequest = {
  scopes: appConfig.azureAdScopes,
};
