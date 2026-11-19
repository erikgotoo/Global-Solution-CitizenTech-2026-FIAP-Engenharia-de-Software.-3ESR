using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zeladoria.Core.Interfaces;
using Zeladoria.Infrastructure.Repositories;

namespace Zeladoria.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ZeladoriaDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IHistoricoStatusRepository, HistoricoStatusRepository>();
        return services;
    }
}
