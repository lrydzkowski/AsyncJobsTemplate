import { apiClient } from './api-client';

export const jobsClient = {
  async getJob(accessToken: string, jobId: string) {
    const { data, error } = await apiClient.GET('/jobs/{jobId}', {
      params: {
        path: { jobId },
      },
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    return { data, error };
  },
};
