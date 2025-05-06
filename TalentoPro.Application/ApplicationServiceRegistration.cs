using Microsoft.Extensions.DependencyInjection;
using TalentoPro.Application.Servicios;
using TalentoPro.Application.Validaciones.Empleado;

namespace TalentoPro.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar validadores
            services.AddScoped<ValidadorEmpleado>();

            // Registrar servicios de aplicación
            services.AddScoped<IEmpleadoServicio, EmpleadoServicio>();

            return services;
        }
    }
} 