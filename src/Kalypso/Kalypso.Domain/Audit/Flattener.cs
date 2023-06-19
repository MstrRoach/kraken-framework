using Dottex.Kalypso.Domain.Core;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Dottex.Kalypso.Domain.Audit;

public class Flattener<T> where T : IAggregate
{
    private List<PropertyInfo> Properties = new();
    private List<FieldInfo> Fields = new();

    public Flattener()
    {
        Initialize();
    }

    /// <summary>
    /// Inicializa el aplanado de las entidades
    /// </summary>
    public void Initialize()
    {
        var entityType = typeof(T);
        GetProperties(entityType, in Properties);
        //GetFields(entityType, in Fields);
    }

    private string FlattenTemplate = "$Entity__$Property";

    private string GetKey(object entity, PropertyInfo property)
        => FlattenTemplate
            .Replace("$Entity", entity.GetType().Name)
            .Replace("$Property", property.Name);

    public JsonObject Flatten(object entity, in JsonObject container)
    {
        // Si la entidad es nula salimos
        if (entity is null)
            return container;
        // Recorremos todas las propiedades recogidas antes
        foreach (var property in entity.GetType().GetProperties())
        {
            // Si es una coleccion
            if (IsCollection(property, typeof(IReadOnlyCollection<>)))
            {
                // Obtenemos el valor de la coleccion
                var collection = property.GetValue(entity);
                // Obtenemos el tipo de la coleccion
                var collectionType = property.PropertyType.GenericTypeArguments[0];
                // Ejecutamos el metodo por reflexion y obtenemos los valores
                var flatArray = this.GetType().GetMethod("GetCollection", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(collectionType).Invoke(this, new object[] { collection }) as JsonArray;
                // Agregamos los datos
                container.Add(
                    GetKey(entity,property), 
                    flatArray
                    );
                // Continuamos
                continue;
            }
            // Si no es util entonces continuamos
            if (!IsUseful(property))
                continue;
            // Si es una enumeracion
            if (IsEnumeration(property))
            {
                // Obtenemos la enumeracion
                var enumeration = property.GetValue(entity);
                // Obtenemos el valor de display
                var displayNameProp = property
                    .PropertyType
                    .GetProperty("DisplayName");
                // Agregamos al diccionario
                container.Add(
                    GetKey(entity, property),
                    JsonValue.Create(displayNameProp.GetValue(enumeration)) 
                    );
                // Continuamos
                continue;
            }
            // Si es una propiedad primitiva
            if (IsPrimitiveType(property))
            {
                container.Add(
                    GetKey(entity, property),
                    JsonValue.Create(property.GetValue(entity))
                    );
                continue;
            }
            // Obtenemos el vaalor del objeto
            var value = property.GetValue(entity);
            // Si es nulo continuamos con lo siguiente
            if (value is null)
                continue;
            // Si no es ninguna entonces es un objeto anidado
            var entityFlatten = Flatten(value, container);
        }
        return container;
    }

    private JsonArray GetCollection<ArrayType>(IReadOnlyCollection<ArrayType> items)
    {
        var elements = new JsonArray();
        foreach(var item in items)
        {
            var flatten = Flatten(item, new JsonObject());
            elements.Add(flatten);
        }
        return elements;
    }

    /// <summary>
    /// Obtiene las propiedades aninadas en una lista plana
    /// </summary>
    /// <param name="wrapper"></param>
    /// <param name="properties"></param>
    private void GetProperties(Type wrapper,
        in List<PropertyInfo> properties)
    {
        // Recorremos las propiedades
        foreach (var property in wrapper.GetProperties())
        {
            // Si es una lista tiene tratamiento especial
            if (IsCollection(property, typeof(IReadOnlyCollection<>)))
            {
                var paramType = property.PropertyType.GenericTypeArguments[0];
                GetProperties(paramType, properties);
                continue;
            }
            // Si no es util para leer o escribir
            if (!IsUseful(property)) continue;
            // Si es enumeracion
            if (IsEnumeration(property))
            {
                // Extraemos la propiedad para ver el texto
                properties.Add(property.PropertyType.GetProperty("DisplayName"));
                continue;
            }

            // Si es primitivo o pertenece a System
            if (IsPrimitiveType(property))
            {
                // Creamos el contenedor de propiedades
                properties.Add(property);
                continue;
            }
            // Llamamos a recursion para el tipo personalizado
            GetProperties(property.PropertyType, properties);
        }
    }

    private void GetFields(Type wrapper, in List<FieldInfo> fields)
    {
        foreach (var field in wrapper.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            // Si no es un tipo definido por el usuario
            if (!field.Name.StartsWith("_"))
                continue;
            // Si es una lista
            if (IsList(field))
            {
                // Extraer la clase generica
                var type = field.FieldType.GenericTypeArguments[0];
                continue;
            }

            Console.WriteLine(field);
        }
    }

    /// <summary>
    /// Define la utilidad de una propiedad basada en si puede leer y escribirse
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool IsUseful(PropertyInfo property) => property.CanRead 
        && property.CanWrite;

    /// <summary>
    /// Indica si una propiedad es base, es decir parte de los primitivos o
    /// parte de las propiedades base de system
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool IsPrimitiveType(PropertyInfo property)
        => property.PropertyType.IsPrimitive 
        || property.PropertyType.FullName.StartsWith("System");

    /// <summary>
    /// Indica si una propiedad es de tipo enumeracion
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool IsEnumeration(PropertyInfo property)
        => property.PropertyType.BaseType is not null 
        && property.PropertyType.BaseType.IsGenericType
        && property.PropertyType.BaseType
        .GetGenericTypeDefinition() == typeof(Enumeration<,>);

    /// <summary>
    /// Indica si un campo es lista
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private bool IsList(FieldInfo field)
        => field.FieldType.IsGenericType
        && field.FieldType.GetGenericTypeDefinition() == typeof(List<>);

    /// <summary>
    /// Permite verificar si una propiedad es un tipo de  colleccion
    /// </summary>
    /// <param name="property"></param>
    /// <param name="CollectionType"></param>
    /// <returns></returns>
    private bool IsCollection(PropertyInfo property, Type CollectionType)
        => property.PropertyType.IsGenericType
        && property.PropertyType.GetGenericTypeDefinition() == CollectionType 
        && property.PropertyType.GenericTypeArguments[0].IsClass;
}