using Kraken.Host;

namespace KrakenExample.Features
{
    /// <summary>
    /// Contiene las extensiones para agregar features del
    /// usuario a la aplicacion kraken
    /// </summary>
    public static class FeatureExtensions
    {

        /// <summary>
        /// Agrega la authrozizacion configurada por el usuario
        /// </summary>
        /// <param name="appDescriptor"></param>
        /// <returns></returns>
        public static AppDescriptor AddCustomAuthorization(this AppDescriptor appDescriptor)
        {
            appDescriptor.Authorization = new CustomAuthorizationFeature();
            return appDescriptor;
        }

        /// <summary>
        /// Agrega la autenticacion definida por el usuario para limitar quien entra o no al
        /// sistema
        /// </summary>
        /// <param name="appDescriptor"></param>
        /// <returns></returns>
        public static AppDescriptor AddCustomAuthentication(this AppDescriptor appDescriptor)
        {
            appDescriptor.Authentication = new CustomAuthenticationFeature();
            return appDescriptor;
        }
    }
}
