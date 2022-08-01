namespace Kraken.Core.Processing;

/// <summary>
/// Permite inicializar y contener las operaciones en segundo plano
/// </summary>
public interface IBootstrapper
{
    /// <summary>
    /// Inicia el procesamiento
    /// </summary>
    /// <returns></returns>
    Task RunAsync();
}
