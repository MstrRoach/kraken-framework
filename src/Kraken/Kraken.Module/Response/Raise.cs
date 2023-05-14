using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Response;

/// <summary>
/// Wrapper para envolver la respuesta en un objeto que acepta como fallo
/// un modelo de tipo error
/// </summary>
/// <typeparam name="Success"></typeparam>
public struct Raise<Success>
{
    /// <summary>
    /// Contiene el tipo principal del metodo
    /// </summary>
    private readonly Success? _success;

    /// <summary>
    /// Obtiene el tipo alterno del metodo
    /// </summary>
    private readonly Error? _fail;

    /// <summary>
    /// Indica si la respuesta es exitosa
    /// </summary>
    private readonly bool _isSuccess;

    /// <summary>
    /// Constructor para cuando la respuesta es exitosa
    /// </summary>
    /// <param name="success"></param>
    private Raise(Success success)
    {
        _success = success;
        _fail = default;
        _isSuccess = true;
    }

    /// <summary>
    /// Constructor para devolver una respuesta erronea
    /// </summary>
    /// <param name="error"></param>
    private Raise(Error error)
    {
        _success = default;
        _fail = error;
        _isSuccess = false;
    }

    /// <summary>
    /// Nos permmite devolver tipos implicitos para exito
    /// </summary>
    /// <param name="left"></param>
    public static implicit operator Raise<Success>(Success left)
    {
        return new Raise<Success>(left);
    }

    /// <summary>
    /// Nos permmite devolver tipos implicitos para error
    /// </summary>
    /// <param name="left"></param>
    public static implicit operator Raise<Success>(Error error)
    {
        return new Raise<Success>(error);
    }

    /// <summary>
    /// Permite ejecutar diferentes operaciones dependiendo si es exitosa
    /// o no la respuesta
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public T Match<T>(Func<Success, T> success, Func<Error, T> error)
    {
        return _isSuccess ? success(_success) : error(_fail);
    }

    /// <summary>
    /// Permite ejecutar una accion de exito o una accion de fallo
    /// segun sea el caso, sin retornar ningun valor
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public void Match(Action<Success> success, Action<Error> error)
    {
        if (_isSuccess)
        {
            success(_success);
            return;
        }
        error(_fail);
    }
}
