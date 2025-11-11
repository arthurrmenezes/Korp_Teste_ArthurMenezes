using Microsoft.Extensions.DependencyInjection;
using StockService.Application.Services;
using StockService.Application.Services.Interfaces;

namespace StockService.Application;

public static class DependencyInjection
{
    public static IServiceCollection ApplyApplicationDependenciesConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductService, ProductService>();

        return serviceCollection;
    }
}
