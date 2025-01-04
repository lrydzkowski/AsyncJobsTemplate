using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Infrastructure;
using AsyncJobsTemplate.Shared;
using AsyncJobsTemplate.WebApi;
using AsyncJobsTemplate.WebApi.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddSharedServices();
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerBasicAuth();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}
