using Kraken.Standard.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Operation.Transaction;

internal static class TransactionExtensions
{
    public static IServiceCollection AddTransaction(this IServiceCollection services)
    {
        // Ya deberia de haberse registrado el middleware de transaccion
        // Registramos la fabrica para la unidad de trabajo
        services.AddScoped<IUnitWorkFactory, DefaultUnitWorkFactory>();
        // Registramos la unidad de trabajo generica para cuando no haya registrado
        services.AddScoped(typeof(DefaultUnitWork<>));
        return services;
    }
}
