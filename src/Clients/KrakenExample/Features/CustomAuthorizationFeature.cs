using Kraken.Core.Features;
using System.Security.Claims;

namespace KrakenExample.Features
{
    public class CustomAuthorizationFeature : IAuthorizationFeature
    {
        /// <summary>
        /// Registra las politicas existentes en la aplicacion
        /// </summary>
        /// <param name="services"></param>
        public void AddServices(IServiceCollection services)
        {
            // Constructor de las politicas de acceso a los endpoint
            services.AddAuthorization(options =>
            {
                // Para ser cliente debe de:
                // - Tener id de usuario, cualquiera
                // - Existir el claim de Tenant
                // - Existir el claim de Rol
                // - El tenant debe ser desconocido, al no tener ninguno
                // - El Rol debe ser desconocido, al no tener ninguno (En el futuro puede ser uno estatico)
                options.AddPolicy("ShouldBeCustomer", policy =>
                {
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == "http://schemas.kazam.com/ws/2022/03/identity/claims/tenant"));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.Role));
                    policy.RequireAssertion(context => context.User.HasClaim("http://schemas.kazam.com/ws/2022/03/identity/claims/tenant", "unknow"));
                    //policy.RequireAssertion(context => context.User.HasClaim(ClaimTypes.Role, "unknow"));
                });

                // Para ser trabajador de taller debe de:
                // - Tener id de usuario, cualquiera
                // - Existir el claim de tenant
                // - Existir el claim de Rol
                // - El tenant debe ser conocido, diferente a unknow
                // - El Rol debe ser conocido, diferente a unknow
                options.AddPolicy("ShouldBeGarageWorker", policy =>
                {
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == "http://schemas.kazam.com/ws/2022/03/identity/claims/tenant"));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.Role));
                    policy.RequireAssertion(context => !context.User.HasClaim("http://schemas.kazam.com/ws/2022/03/identity/claims/tenant", "unknow"));
                    policy.RequireAssertion(context => !context.User.HasClaim(ClaimTypes.Role, "unknow"));
                });

                // Para ser trabajador de kazam debe de:
                // - Tener id de usuario, cualquiera
                // - Existir el claim de tenant
                // - Existir el claim de Rol
                // - El tenant debe ser desconocido, al no tener ninguno
                // - El Rol debe ser conocido, diferente a unknow
                options.AddPolicy("ShouldBeKazamWorker", policy =>
                {
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == "http://schemas.kazam.com/ws/2022/03/identity/claims/tenant"));
                    policy.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == ClaimTypes.Role));
                    policy.RequireAssertion(context => context.User.HasClaim("http://schemas.kazam.com/ws/2022/03/identity/claims/tenant", "unknow"));
                    policy.RequireAssertion(context => !context.User.HasClaim(ClaimTypes.Role, "unknow"));
                });
            });
        }

        /// <summary>
        /// Registra la authorizacion en el pipeline de la aplicacion
        /// </summary>
        /// <param name="app"></param>
        public void UseServices(IApplicationBuilder app)
        {
            app.UseAuthorization();
        }
    }
}
