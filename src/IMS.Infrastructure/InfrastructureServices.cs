using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
