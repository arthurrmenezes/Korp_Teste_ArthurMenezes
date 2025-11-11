using BillingService.Infrastructure.Data;
using BillingService.Infrastructure.Data.Repositories;
using BillingService.Infrastructure.Data.Repositories.Interfaces;
using BillingService.Infrastructure.Services.External.StockService;
using BillingService.Infrastructure.Services.External.StockService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace BillingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ApplyInfrastructureDependencyInjection(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DataContext>(options => options.UseNpgsql(
            connectionString: configuration["Database:ConnectionString"]));

        serviceCollection.AddScoped<IInvoiceRepository, InvoiceRepository>();

        # region Retry Configuration

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        # endregion

        serviceCollection.AddHttpClient<IStockService, StockService>(externalService =>
        {
            var baseUrl = configuration["StockService:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("The base url from Stock Service was not found.");

            externalService.BaseAddress = new Uri(baseUrl);
        }).AddPolicyHandler(retryPolicy);

        return serviceCollection;
    }
}
