using Dottex.Kalypso.Domain;
using Dottex.Kalypso.Module.Common;

namespace Dottex.Domain.AuditStorage.InMemory;

/// <summary>
/// Extension para registrar el servicio de almacen en memoria
/// </summary>
public static class AuditStorageExtensions
{
    public static DomainDrivenDesignOptions<TModule> UseInMemoryAuditStorage<TModule>(
        this DomainDrivenDesignOptions<TModule> extensions, 
        Action<InMemoryAuditStorageOptions<TModule>> inMemoryOptions = null)
        where TModule : IModule
    {
        var options = new InMemoryAuditStorageOptions<TModule>();
        inMemoryOptions?.Invoke(options);
        var extension = new InMemoryAuditStorageExtensions<TModule>(options);
        extensions.RegisterAuditStorage(extension);
        return extensions;
    }
}