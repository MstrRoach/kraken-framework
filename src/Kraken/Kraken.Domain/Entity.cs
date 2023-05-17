using System;
using System.Collections.Generic;
using System.Text;

namespace Kraken.Domain;

/// <summary>
/// Clase base para marcar una clase como una
/// entiddad de negocio de acuerdo a Domain
/// Driven Design
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
public abstract class Entity<TEntity, TIdentifier>
    where TIdentifier : IComparable
{
    /// <summary>
    /// Indica un status especifico para las entidades
    /// </summary>
    public enum EntityStatus { Clean, Added, Updated, Deleted };

    /// <summary>
    /// Id con el tipo especificado el tipo de entidad
    /// </summary>
    public TIdentifier Id { get; protected set; }


    /// <summary>
    /// Metodo de equals para entidades
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (!(obj is Entity<TEntity, TIdentifier> other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id.Equals(default(Type)) || other.Id.Equals(default(Type)))
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Operador de equals para entidades
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(Entity<TEntity, TIdentifier> a, Entity<TEntity, TIdentifier> b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Operador de nonequal para entidades
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(Entity<TEntity, TIdentifier> a, Entity<TEntity, TIdentifier> b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Generacion del codigo hash para identificacion de entidades
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Id.GetHashCode();

}
