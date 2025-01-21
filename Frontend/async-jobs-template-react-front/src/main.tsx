import '@mantine/core/styles.css';
import './main.css';

import { StrictMode } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { AuthenticatedTemplate, MsalProvider, UnauthenticatedTemplate } from '@azure/msal-react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Route, Routes } from 'react-router';
import { MantineProvider } from '@mantine/core';
import { App } from './app.tsx';
import { msalConfig } from './auth/msal-config.ts';
import { HomePage } from './pages/home-page.tsx';
import { LoginPage } from './pages/login-page.tsx';
import NotFoundPage from './pages/not-found-page.tsx';

const msalInstance = new PublicClientApplication(msalConfig);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineProvider>
      <MsalProvider instance={msalInstance}>
        <UnauthenticatedTemplate>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<App />}>
                <Route index element={<LoginPage />} />
                <Route path="*" element={<NotFoundPage />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </UnauthenticatedTemplate>
        <AuthenticatedTemplate>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<App />}>
                <Route index element={<HomePage />} />
                <Route path="*" element={<NotFoundPage />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </AuthenticatedTemplate>
      </MsalProvider>
    </MantineProvider>
  </StrictMode>,
);
