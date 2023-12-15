using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module;
using Dottex.Kalypso.Module.Audit;
using Dottex.Kalypso.Module.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Decorador encargado de obtener los valores por cada
/// uno de las propiedades del agregado y calcular el delta
/// aplicado para crear los logs de auditoria
/// </summary>
public sealed class AuditorRepositoryExtractor<T> : IRepository<T>
    where T : IAggregate, IAuditable
{
    /// <summary>
    /// Repositorio interno que contiene las lecturas
    /// y escrituras
    /// </summary>
    private readonly IRepository<T> _inner;

    /// <summary>
    /// Servicio para aplanar las entidades
    /// </summary>
    //readonly Flattener<T> _flattener;

    /// <summary>
    /// Extractor de cambios 
    /// </summary>
    //readonly ChangeExtractor _changeExtractor;

    readonly DifferenceExtractor _extractor;

    /// <summary>
    /// Almacen para auditoria
    /// </summary>
    readonly IAuditStorage _auditStorage;

    /// <summary>
    /// Contexto de la aplicacion
    /// </summary>
    readonly IContext _context;

    /// <summary>
    /// Constructor del decorador
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="flattener"></param>
    /// <param name="changeExtractor"></param>
    /// <param name="auditStorage"></param>
    /// <param name="context"></param>
    public AuditorRepositoryExtractor(IRepository<T> inner,
        //Flattener<T> flattener, 
        //ChangeExtractor changeExtractor,
        DifferenceExtractor extractor,
        IAuditStorage auditStorage, 
        IContext context)
    {
        _inner = inner;
        //_flattener = flattener;
        //_changeExtractor = changeExtractor;
        _extractor = extractor;
        _auditStorage = auditStorage;
        _context = context;
    }

    /// <summary>
    /// Interviene el proceso de creacion para calcular el delta contra
    /// una entidad nueva sin ningun valor establecido
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Create(T aggregate)
    {
        await _inner.Create(aggregate);
        // Creacion de instancia por defecto
        var oldEntity = Activator.CreateInstance(typeof(T),true);
        // Aplanando las entidades
        //var oldEntityFlat = _flattener.Flatten(oldEntity, new JsonObject());
        var oldEntityDocument = _extractor.GetJsonRepresentation(oldEntity);
        //var newEntityFlat = _flattener.Flatten(aggregate, new JsonObject());
        var newEntityDocument = _extractor.GetJsonRepresentation(aggregate);
        // Extrayendo cambios entre entidades
        //var delta = _changeExtractor.GetChanges(oldEntityFlat, newEntityFlat);
        var delta = _extractor.GetDelta(oldEntityDocument.RootElement, newEntityDocument.RootElement);
        // Setteamos el nuevo valor del estado
        SetState(aggregate, newEntityDocument);
        // Creando el registro de auditoria
        var auditlog = new AuditLog
        {
            Module = typeof(T).GetModuleName(),
            EntityId = aggregate.AggregateId,
            Entity = aggregate.AggregateRootType,
            Operation = Enum.GetName<AuditOperation>(AuditOperation.Create),
            Delta = delta,
            User = _context is not null ? _context.Identity.Name : "System",
            UpdatedAt = DateTime.UtcNow
        };
        // Guardando el registro de auditoria
        _auditStorage.Save(auditlog);
    }

    private void SetState(T aggregate, JsonDocument state)
    {
        var aggregateType = aggregate.GetType();
        var field = aggregateType.BaseType.GetField("_state",
            BindingFlags.Public 
            | BindingFlags.NonPublic
            | BindingFlags.Instance);
        field.SetValue(aggregate, state);
    }

    /// <summary>
    /// Intercepta la eliminacion y realiza la auditoria
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Delete(T aggregate)
    {
        await _inner.Delete(aggregate);
        // Aplanamos la instancia entrande
        //var newEntityFlat = _flattener.Flatten(aggregate, new JsonObject());
        var newEntityDocument = _extractor.GetJsonRepresentation(aggregate);
        // Calculamos el incremento
        //var delta = _changeExtractor.GetChanges(aggregate.State, newEntityFlat);
        var delta = _extractor.GetDelta(aggregate.State.RootElement,newEntityDocument.RootElement);
        // Si el incremento es cero
        if(delta is null)
        {
            //delta = _changeExtractor.GetSnapshot(newEntityFlat);
            delta = _extractor.GetDelta<T>(aggregate, default);
        }
        // Creando el registro de auditoria
        var auditlog = new AuditLog
        {
            Module = typeof(T).GetModuleName(),
            EntityId = aggregate.AggregateId,
            Entity = aggregate.AggregateRootType,
            Operation = Enum.GetName<AuditOperation>(AuditOperation.Delete),
            Delta = delta,
            User = _context is not null ? _context.Identity.Name : "System",
            UpdatedAt = DateTime.UtcNow
        };
        // Guardando el registro de auditoria
        _auditStorage.Save(auditlog);
    }

    /// <summary>
    /// Intercepta la obtencion de un agregado y calcula el estado
    /// para setearlo
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public async Task<T> Get(ISpecification<T> specification)
    {
        // Recuperamos el aggregado
        var aggregate = await _inner.Get(specification);
        // Si es nulo lo devolvemos
        if (aggregate == null)
            return aggregate;
        // Creamos el estado
        //var state = _flattener.Flatten(aggregate, new JsonObject());
        var state = _extractor.GetJsonRepresentation(aggregate);
        // Establecemos el estado
        SetState(aggregate, state);
        // DEvolvemos el agregado
        return aggregate;
    }

    /// <summary>
    /// Intercepta la consulta para crear el estado de cada uno
    /// de los agregados
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public async Task<List<T>> GetAll(ISpecification<T> specification)
    {
        // Obtenemos los agregados
        var aggregates = await _inner.GetAll(specification);
        // Si la lista esta vacia la retornamos
        if(aggregates.Count < 1)
            return aggregates;
        // Recorremos los agregados
        foreach ( var aggregate in aggregates)
        {
            // Creamos el estado
            //var state = _flattener.Flatten(aggregate, new JsonObject());
            var state = _extractor.GetJsonRepresentation(aggregate);
            // Establecemos el estado
            SetState(aggregate, state);
        }
        // Devolvemos la lista
        return aggregates;
    }

    /// <summary>
    /// Intercepta la actualizacion para generar el estado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Update(T aggregate)
    {
        await _inner.Update(aggregate);
        //var oldEntityFlat = flattenEntities[aggregate.AggregateId] as JsonObject;
        // Obteniendo el estado actual del agregado
        //var newEntityFlat = _flattener.Flatten(aggregate, new JsonObject());
        var newEntityDocument = _extractor.GetJsonRepresentation(aggregate);
        //var test = _extractor.GetJsonRepresentation(aggregate);
        // Calculando el incremento
        //var delta = _changeExtractor.GetChanges(aggregate.State, newEntityFlat);
        var delta = _extractor.GetDelta(aggregate.State, newEntityDocument);
        // Setteamos el nuevo valor del estado
        SetState(aggregate, newEntityDocument);
        // Si no hay cambio, salimos
        if (delta is null)
            return;
        // Creando el cambio
        var auditlog = new AuditLog
        {
            Module = typeof(T).GetModuleName(),
            EntityId = aggregate.AggregateId,
            Entity = aggregate.AggregateRootType,
            Operation = Enum.GetName<AuditOperation>(AuditOperation.Update),
            Delta = delta,
            User = _context is not null ? _context.Identity.Name : "System",
            UpdatedAt = DateTime.UtcNow
        };
        // Guardando
        _auditStorage.Save(auditlog);
    }
}
