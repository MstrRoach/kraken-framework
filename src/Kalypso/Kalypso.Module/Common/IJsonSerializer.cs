using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Common;

public interface IJsonSerializer
{
    /// <summary>
    /// Convierte un objeto en una cadena en formato json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    string Serialize<T>(T value);

    /// <summary>
    /// Deserializa una cadena en el valor especificado por
    /// el parametro generico
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    T Deserialize<T>(string value);

    /// <summary>
    /// Deserializa una cadena en el tipo especificado
    /// por el parametro type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    object Deserialize(string value, Type type);
}
