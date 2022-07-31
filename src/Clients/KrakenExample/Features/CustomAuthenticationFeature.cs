using Kraken.Core.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KrakenExample.Features
{
    public class CustomAuthenticationFeature : IAuthenticationFeature
    {
        /// <summary>
        /// Agrega los servicios de autenticacion
        /// </summary>
        /// <param name="services"></param>
        public void AddServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireSignedTokens = false,
                        ValidAudience = "Kraken",
                        ValidIssuer = "Kraken",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Yh2k7QSu4l8CZg5p6X3Pna9L0Miy4D3Bvt0JVr87UcOj69Kqw5R2Nmf4FWs03Hdx"))
                    };

                });
        }

        /// <summary>
        /// Inyecta los servicios necesarios en la parte de
        /// autenticacion
        /// </summary>
        /// <param name="app"></param>
        public void UseServices(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
