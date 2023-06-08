using Domain.Repository.InMemory.MemoryStorable;
using Kraken.Module.Common;

namespace Domain.Repository.InMemory;

/// <summary>
/// Clase para inicializar la base de datos y cadda uno de
/// las tablas
/// </summary>
/// <typeparam name="TModule"></typeparam>
public class DatabaseBootstrap<TModule> where TModule : IModule
{
    /// <summary>
    /// Opciones de configuracion para la base de datos
    /// </summary>
    readonly InMemoryRepositoryOptions<TModule> _options;

    /// <summary>
    /// Lista de almacenes en memoria del modulo
    /// </summary>
    readonly List<IMemoryStorable<TModule>> _storables;

    /// <summary>
    /// Constructor del arrancador de la base de datos en memoria
    /// </summary>
    /// <param name="options"></param>
    /// <param name="storables"></param>
    public DatabaseBootstrap(InMemoryRepositoryOptions<TModule> options, 
        IEnumerable<IMemoryStorable<TModule>> storables)
    {
        _options = options;
        _storables = storables.ToList();
    }

    /// <summary>
    /// Inicializa la base de datos
    /// </summary>
    public async Task Initialize()
    {
        await EnsureDatabase();
        await EnsureTables();
    }

    /// <summary>
    /// Asegura que el archivo de base de datos exista
    /// </summary>
    private Task EnsureDatabase()
    {
        var database = $"{_options.DatabaseName}.db";
        var databasePath = Path.Combine(
            _options.ApplicationData, 
            _options.ApplicationName,
            database);
        var databaseFolder = Path.Combine(
            _options.ApplicationData, 
            _options.ApplicationName);
        if (File.Exists(databasePath))
            return Task.CompletedTask;
        if (!Directory.Exists(databaseFolder))
            Directory.CreateDirectory(databaseFolder);
        File.Create(path: databasePath);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asegura que los esquemas esten bien realizados
    /// </summary>
    private async Task EnsureTables()
    {
        foreach (var item in _storables)
        {
            await item.Initialize();
        }
    }
}
