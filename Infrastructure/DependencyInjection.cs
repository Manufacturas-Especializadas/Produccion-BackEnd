using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.FileStorage;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Connection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IMasterIndustrialRepository, MasterIndustrialRepository>();
            services.AddScoped<IDemandPlanRepository, DemandPlanRepository>();
            services.AddScoped<IHourlyProductionRepository,  HourlyProductionRepository>();
            services.AddScoped<IDowntimeCodeRepository, DowntimeCodeRepository>();
            services.AddScoped<IExcelParserService, ExcelParserService>();

            return services;
        }

    }
}