﻿using Kraken.Domain.Core;
using Kraken.Domain.Storage;
using Kraken.Module;
using Kraken.Module.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Domain;

public static class DomainExtensions
{
    /// <summary>
    /// Registra y habilita los mecanismos para utilizar domain driven design, incluyendo el uso
    /// del interceptor para obtener los eventos de los agregados y distribuirlos
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomainDrivenDesign(this IServiceCollection services, Action<DomainDrivenDesignOptions> builder)
    {
        // Validamos que las opciones tengan lo obligatorio
        var options = new DomainDrivenDesignOptions();
        // Las precargamos
        builder(options);
        // Validamos que este todo lo necesario
        options.Validate();
        var callerAssembly = Assembly.GetCallingAssembly();
        // Extraemos todos los agregados
        var callerAggregates = callerAssembly.GetTypes()
            .Where(x => !x.IsOpenGeneric())
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetInterface(nameof(IAggregate)) is not null)
            .ToList();
        // Registramos los repositorios
        options.RepositoryExtension.AddServices(services,callerAggregates);
        // Registramos el decorador
        services.TryDecorate(typeof(IRepository<>), typeof(EventExtractorRepositoryDecorator<>));
        // Salimos
        return services;
    }
}