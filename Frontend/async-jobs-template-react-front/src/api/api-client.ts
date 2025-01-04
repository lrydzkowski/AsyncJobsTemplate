import createClient from 'openapi-fetch';
import type { paths } from '../../types/async-jobs-template-api-types';
import appConfig from '../config/app-config';

export const apiClient = createClient<paths>({ baseUrl: appConfig.apiBaseUrl });
