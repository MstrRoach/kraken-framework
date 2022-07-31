using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Features
{
    /// <summary>
    /// Interface para anclar el feature de autenticacion y permitir al
    /// usuario implementar su propia estrategia
    /// </summary>
    public interface IAuthenticationFeature : IKrakenFeature
    {
    }
}
