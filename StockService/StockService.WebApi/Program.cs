using StockService.Application;
using StockService.Infrastructure;
using StockService.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyInfrastructureDependencyInjection(builder.Configuration["Database:ConnectionString"]!);

builder.Services.ApplyApplicationDependenciesConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
