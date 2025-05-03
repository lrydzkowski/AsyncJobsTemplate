using AsyncJobsTemplate.Core.Commands.RunJob;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Consumers;
using MediatR;

namespace AsyncJobsTemplate.WebApi.Consumers;

public class JobsQueueConsumer : TypedMessageConsumer<JobMessage>
{
    private readonly IMediator _mediator;

    public JobsQueueConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task ConsumeMessageAsync(JobMessage data)
    {
        await _mediator.Send(new RunJobCommand { Request = new RunJobRequest { JobId = data.JobId } });
    }
}
