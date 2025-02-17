import { sleep } from 'k6';
import { Options } from 'k6/options';
import { host } from './models/test-data.js';
import { user1AccessToken } from './models/test-data.js';
import { getJob, getJobs, triggerJob } from './requests.js';
// @ts-expect-error Import module
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
// @ts-expect-error Import module
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

export const options: Options = {
  scenarios: {
    scenario1: {
      executor: 'ramping-vus',
      exec: 'scenario1',
      stages: [
        { duration: '20s', target: 40 },
        { duration: '2m', target: 40 },
        { duration: '10s', target: 0 },
      ],
    },
  },
};

export function scenario1() {
  triggerJob(host, 'job1', user1AccessToken);
  sleep(randomIntBetween(1, 3) * 0.1);

  const jobIds = getJobs(host, user1AccessToken, 1, 5);
  sleep(randomIntBetween(1, 3) * 0.1);

  if (jobIds.length > 0) {
    getJob(host, user1AccessToken, jobIds[0]);
    sleep(randomIntBetween(1, 3) * 0.1);
  }
}

export const handleSummary = function (data: unknown) {
  return {
    './results/summary.html': htmlReport(data, { title: new Date().toLocaleString() }),
    './results/summary.json': JSON.stringify(data),
  };
};
