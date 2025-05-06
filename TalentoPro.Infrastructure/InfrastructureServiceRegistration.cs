using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentoPro.Domain.Repositories;
using TalentoPro.Infrastructure.Data;
using TalentoPro.Infrastructure.Repositories;

namespace TalentoPro.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar servicios de acceso a datos
            services.AddSingleton<ConexionBD>();
            services.AddScoped<DBContext>();

            // Registrar repositorios
            services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();

            return services;
        }
    }
} 