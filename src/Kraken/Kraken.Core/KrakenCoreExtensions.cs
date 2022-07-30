using Kraken.Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core
{
    public static class KrakenCoreExtensions
    {
        /// <summary>
        /// Obtiene el nombre del modulo al que pertenece el comando
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetModuleName(this object value)
            => value?.GetType().GetModuleName() ?? string.Empty;


        /// <summary>
        /// Obtiene el nombre del modulo revisando el nombre del namespace del comando
        /// </summary>
        /// <param name="type"></param>
        /// <param name="namespacePart"></param>
        /// <param name="splitIndex"></param>
        /// <returns></returns>
        public static string GetModuleName(this Type type, int splitIndex = 0)
        {
            if (type?.Namespace is null)
                return string.Empty;
            // Si el tipo es intancia de module event
            if(typeof(IModuleEvent).IsAssignableFrom(type))
                splitIndex = 1;

            return type.Namespace.Split(".")[splitIndex].ToLowerInvariant();
        }

        /// <summary>
        /// Busca las interfaces cerradass que coinciden con la interface abierta proporcionada
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
        }

        /// <summary>
        /// Realiza la busqueda recursiva en todos los tipos derivados para encontrar las interfaces
        /// cerradas que cummplen con la interface abierta
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
        {
            if (pluggedType == null) yield break;

            if (!pluggedType.IsConcrete()) yield break;

            if (templateType.GetTypeInfo().IsInterface)
            {
                foreach (
                    var interfaceType in
                    pluggedType.GetInterfaces()
                        .Where(type => type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                     (pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
            {
                yield return pluggedType.GetTypeInfo().BaseType;
            }

            if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

            foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
            {
                yield return interfaceType;
            }
        }

        /// <summary>
        /// Indica si el tipo especificado es concreto y si es interface
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConcrete(this Type type)
        {
            return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
        }

        /// <summary>
        /// Indica si el tipo es un generico abierto
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsOpenGeneric(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
        }

        /// <summary>
        /// Obtiene el parametro generico del tipo que implementa la interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openInterface"></param>
        /// <returns></returns>
        public static Type GetHandlerArgument(this Type type, string openInterface)
        {
            return type.GetInterface(openInterface).GetGenericArguments().FirstOrDefault();
        }
    }
}
