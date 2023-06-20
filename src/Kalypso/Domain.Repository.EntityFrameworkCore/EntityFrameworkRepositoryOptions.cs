using Microsoft.EntityFrameworkCore;

namespace Dottex.Domain.Repository.EntityFrameworkCore;

/// <summary>
/// Configuracion para la funcionalidad de repositorios con efcore
/// </summary>
/// <typeparam name="TModule"></typeparam>
public class EntityFrameworkRepositoryOptions<TModule>
{
    /// <summary>
    /// Contiene el tipo de contexto de efcore
    /// </summary>
    internal Type ModuleDbContext;

    /// <summary>
    /// Agregaa el contexto para usarlo en la generacion 
    /// de todos los repositorios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void UseContext<T>()
        where T : DbContext
    {
        ModuleDbContext = typeof(T);
    }
}