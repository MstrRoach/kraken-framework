using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dottex.Kalypso.Domain.Core
{
    /// <summary>
    /// Clase base que define a un enumerador de un tipo especifico
    /// </summary>
    /// <typeparam name="TEnumeration"></typeparam>
    /// <typeparam name="TIdentifier"></typeparam>
    public abstract class Enumeration<TEnumeration, TIdentifier> : IComparable<TEnumeration>, IEquatable<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration, TIdentifier>
        where TIdentifier : IComparable
    {

        /// <summary>
        /// Contiene los enumeradores que extienden de la clase base
        /// </summary>
        public static TEnumeration[] Enumerations { get; private set; } = GetEnumerations();

        /// <summary>
        /// Id de la enumeracion correspondiente
        /// </summary>
        public TIdentifier Id { get; private set; }

        /// <summary>
        /// Nombre para mostrar del elemento de la enumeracion
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Constructor protegido de la clase base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="displayName"></param>
        protected Enumeration(TIdentifier id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        /// <summary>
        /// Recupera el valor de la enumeracion en base al Id del mismo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TEnumeration FromValue(TIdentifier id)
        {
            return Parse(id, "value", item => item.Id.Equals(id));
        }

        /// <summary>
        /// Recupera el valor en base al nombre para mostrar
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static TEnumeration FromDisplayName(string displayName)
        {
            return Parse(displayName, "display name", item => item.DisplayName == displayName);
        }

        /// <summary>
        /// Intenta recuperar el valor e indica si se encontro o no en base al valor del enumerador
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(TIdentifier id, out TEnumeration result)
        {
            return TryParse(x => x.Id.Equals(id), out result);
        }

        /// <summary>
        /// Intenta recuperar el enumerador en base al nombre para mostrar e indica si se encontro o no
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string displayName, out TEnumeration result)
        {
            return TryParse(x => x.DisplayName == displayName, out result);
        }

        /// <summary>
        /// Compara un enumerador con otro utilizando la funcion de comparacion y devolviendo un entero en base a su comparacion
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TEnumeration other)
        {
            return Id.CompareTo(other.Id);
        }

        /// <summary>
        /// Cuando se ocupe saber el valor, se devuelve el nombre para mostrar
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Sobre eescribe el metodo generico de comparacion
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Convertimos a enumerador
            var otherValue = obj as TEnumeration;
            // Verificamos si se pudo
            if (otherValue == null)
                return false;
            // Verificamos la igualdad con el medoto de enumeracion
            return Equals(otherValue);
        }

        /// <summary>
        /// Implementa el metodo para verificar la igualdad de tipos
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Equals([AllowNull] TEnumeration otherValue)
        {
            // Comparamos el tipo
            var typeMatches = GetType().Equals(otherValue.GetType());
            // Comparamos los identificadores
            var valueMatches = Id.Equals(otherValue.Id);
            // Si ambos son validos se devuelve
            return typeMatches && valueMatches;
        }

        /// <summary>
        /// Implementa el metodo para verificar que no sea igual al valor requerido
        /// </summary>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public bool NonEquals(TEnumeration otherValue)
        {
            return !Equals(otherValue);
        }

        /// <summary>
        /// Obtiene el codigo hash del valor del enumerador
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Intenta recuperar el valor en base a un predicado
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParse(Func<TEnumeration, bool> predicate, out TEnumeration result)
        {
            result = Enumerations.FirstOrDefault(predicate);
            return result != null;
        }

        /// <summary>
        /// Recupera los enumerados declarados estaticos para un tipo de enumerados
        /// </summary>
        /// <returns></returns>
        private static TEnumeration[] GetEnumerations()
        {
            var enumerationType = typeof(TEnumeration);

            return enumerationType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => enumerationType.IsAssignableFrom(info.FieldType))
                .Select(info => info.GetValue(null))
                .Cast<TEnumeration>()
                .ToArray();
        }

        /// <summary>
        /// Busca un enumerado que coincida con el predicado que se aplica, lanza excepcion si no se encontro
        /// </summary>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static TEnumeration Parse(object value, string description, Func<TEnumeration, bool> predicate)
        {
            TEnumeration result;

            if (!TryParse(predicate, out result))
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(TEnumeration));
                throw new ArgumentException(message, "value");
            }

            return result;
        }

        /// <summary>
        /// Indica si existe un elemento que coincida con el valor
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool ExistForValue(TIdentifier id)
        {
            var result = Enumerations.FirstOrDefault(item => item.Id.Equals(id));
            return result != null;
        }

    }
}
