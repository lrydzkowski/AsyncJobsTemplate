export type GetJobsResponse = {
  jobs: JobsList;
};

export type JobsList = {
  count: number;
  data: Job[];
};

export type Job = {
  jobId: string;
};
