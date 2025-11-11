using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockService.Infrastructure.Data;
using StockService.Infrastructure.Data.Repositories;
using StockService.Infrastructure.Data.Repositories.Interfaces;

namespace StockService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ApplyInfrastructureDependencyInjection(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        serviceCollection.AddDbContext<DataContext>(options => options.UseNpgsql(
            connectionString: connectionString));

        serviceCollection.AddScoped<IProductRepository, ProductRepository>();

        return serviceCollection;
    }
}
