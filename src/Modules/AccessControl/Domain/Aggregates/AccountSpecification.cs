using Kraken.Domain.Storage;

namespace AccessControl.Domain.Aggregates;

internal class AccountSpecification
{
    /// <summary>
    /// Obtiene una cuenta por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ISpecification<Account> GetById(Guid id)
        => GenericSpecification<Account>.Create(x => x.Id == id);
}