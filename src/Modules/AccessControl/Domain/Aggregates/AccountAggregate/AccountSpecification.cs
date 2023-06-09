using Kraken.Domain.Storage;

namespace AccessControl.Domain.Aggregates.AccountAggregate;

internal class AccountSpecification
{
    /// <summary>
    /// Obtiene una cuenta por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ISpecification<Account> GetById(Guid id)
        => GenericSpecification<Account>.Create(x => x.Id == id);

    /// <summary>
    /// Devuelve todos los registros
    /// </summary>
    /// <returns></returns>
    public static ISpecification<Account> GetAll()
        => GenericSpecification<Account>.Create(x => 1 == 1);
}