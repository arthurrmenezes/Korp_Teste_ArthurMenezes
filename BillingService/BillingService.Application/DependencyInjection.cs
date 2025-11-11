using BillingService.Application.Services;
using BillingService.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BillingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection ApplyApplicationDependencyInjection(
        this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IInvoiceService, InvoiceService>();

        return serviceCollection;
    }
}
