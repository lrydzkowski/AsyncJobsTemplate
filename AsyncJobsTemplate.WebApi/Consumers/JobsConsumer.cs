using AsyncJobsTemplate.Core.Commands.RunJob;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using MassTransit;
using MediatR;

namespace AsyncJobsTemplate.WebApi.Consumers;

public class JobsConsumer : IConsumer<JobMessage>
{
    private readonly IMediator _mediator;

    public JobsConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<JobMessage> context)
    {
        await _mediator.Send(new RunJobCommand { Request = new RunJobRequest { JobId = context.Message.JobId } });
    }
}
