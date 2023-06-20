using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Common;

/// <summary>
/// Modelo para enviar respuestas envueltas
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indica el estado de la operacion, comunmente un error legible
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// Indica los datos en caso de exito, los fallos en caso de fallo o el stactrace en caso de error
    /// </summary>
    public object? Data { get; private set; }

    /// <summary>
    /// Indica un mensaje en caso de error
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Indica un codigo de error en caso de tener un diccionario 
    /// de errores
    /// </summary>
    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Respuesta para cuando es un caso de exito
    /// </summary>
    /// <param name="status"></param>
    /// <param name="data"></param>
    public ApiResponse(object data)
    {
        this.Status = ResponseStatus.success.ToString();
        this.Data = data;
        this.Message = null;
        this.ErrorCode = null;
    }

    /// <summary>
    /// Respuesta para cuando es un fail
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <param name="errorCode"></param>
    public ApiResponse(string message, string errorCode)
    {
        this.Status = ResponseStatus.fail.ToString();
        this.Data = null;
        this.Message = message;
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// Respuesta en caso de excepcion o error fatal
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    public ApiResponse(string message, Exception ex)
    {
        this.Status = ResponseStatus.error.ToString();
        this.Data = ex.Message;
        this.Message = message;
        this.ErrorCode = ex.HResult.ToString();
    }
}

/// <summary>
/// Enumerador para los estados de la operacion
/// </summary>
public enum ResponseStatus
{
    success,
    fail,
    error
}
