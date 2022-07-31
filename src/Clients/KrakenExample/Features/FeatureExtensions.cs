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
    }
}
