import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './main.css';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './auth/msal-config.ts';
import { AuthenticatedTemplate, MsalProvider, UnauthenticatedTemplate } from '@azure/msal-react';
import { BrowserRouter, Route, Routes } from 'react-router';
import { App } from './app.tsx';
import { LoginPage } from './pages/login-page.tsx';
import NotFoundPage from './pages/not-found-page.tsx';
import { HomePage } from './pages/home-page.tsx';

const msalInstance = new PublicClientApplication(msalConfig);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
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
  </StrictMode>,
);
