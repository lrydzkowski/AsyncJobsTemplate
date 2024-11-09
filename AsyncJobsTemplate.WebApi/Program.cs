using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Infrastructure;
using AsyncJobsTemplate.WebApi;
using AsyncJobsTemplate.WebApi.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerBasicAuth();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
