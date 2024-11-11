using AsyncJobsTemplate.Infrastructure.Db.Models;
using JobErrorCore = AsyncJobsTemplate.Core.Common.Models.JobError;

namespace AsyncJobsTemplate.Infrastructure.Db.Mappers;

internal interface IJobMapper
{
    List<JobError> Map(List<JobErrorCore> jobErrors);

    JobError Map(JobErrorCore jobError);

    List<JobErrorCore> Map(List<JobError> jobErrors);

    JobErrorCore Map(JobError jobError);
}

internal class JobMapper
    : IJobMapper
{
    public List<JobError> Map(List<JobErrorCore> jobErrors)
    {
        return jobErrors.Select(Map).ToList();
    }

    public JobError Map(JobErrorCore jobError)
    {
        return new JobError
        {
            ErrorCode = jobError.ErrorCode,
            Message = jobError.Message,
            ExceptionMessage = jobError.Exception?.Message
        };
    }

    public List<JobErrorCore> Map(List<JobError> jobErrors)
    {
        return jobErrors.Select(Map).ToList();
    }

    public JobErrorCore Map(JobError jobError)
    {
        return new JobErrorCore
        {
            ErrorCode = jobError.ErrorCode,
            Message = jobError.Message
        };
    }
}
