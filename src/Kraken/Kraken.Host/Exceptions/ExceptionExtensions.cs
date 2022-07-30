using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Kraken.Core.Exceptions;

namespace Kraken.Host.Exceptions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Agrega la administracion de errores a los servicios disponibles
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddErrorHandling(this IServiceCollection services)
            => services
                .AddScoped<ErrorHandlerMiddleware>()
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>();

        /// <summary>
        /// Usa la administracion en la canalizacion especificada
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}