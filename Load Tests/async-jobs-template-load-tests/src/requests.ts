import { check } from 'k6';
import { Trend } from 'k6/metrics';
import http from 'k6/http';

const requestTrends = {
  getJobs: new Trend('getJobs'),
  getJob: new Trend('getJob'),
};

export const getJobs = (
  host: string,
  accessToken: string,
  page: number | null = null,
  pageSize: number | null = null,
): void => {
  const queryParameters = [];
  if (page !== null) {
    queryParameters.push(`page=${page}`);
  }

  if (pageSize !== null) {
    queryParameters.push(`pageSize=${pageSize}`);
  }

  const url = `${host}/jobs?${queryParameters.join('&')}`;
  const params = {
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };
  const response = http.get(url, params);
  check(response, {
    'getJobs response has status 200': (r) => r.status === 200,
  });
  requestTrends.getJobs.add(response.timings.duration);
};

export const getJob = (host: string, accessToken: string, jobId: string): void => {
  const url = `${host}/jobs/${jobId}`;
  const params = {
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };
  const response = http.get(url, params);
  check(response, {
    'getJob response has status 200': (r) => r.status === 200,
  });
  requestTrends.getJob.add(response.timings.duration);
};
