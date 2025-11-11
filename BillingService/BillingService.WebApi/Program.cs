using BillingService.Application;
using BillingService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyInfrastructureDependencyInjection(builder.Configuration);

builder.Services.ApplyApplicationDependencyInjection();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
