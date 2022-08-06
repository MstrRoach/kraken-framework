using Dapper;
using Kraken.Core.Pagination;
using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Storage;

internal class DefaultRelationalData<M> : IRelationalData<M>
    where M : IModule
{

    /// <summary>
    /// Compilador
    /// </summary>
    private readonly Compiler _compiler;

    /// <summary>
    /// Funcion para construir una conexion de base de datos
    /// </summary>
    private readonly Func<IDbConnection> _connectionFactory;

    /// <summary>
    /// Constructor para los datos relacionales
    /// </summary>
    /// <param name="moduleOptions"></param>
    public DefaultRelationalData(IOptions<RelationalOptions<M>> moduleOptions)
    {
        // Si el compilador o la cadena es nulo, lanzamos error
        _connectionFactory = moduleOptions.Value.ConnectionFactory 
            ?? throw new ArgumentException(nameof(moduleOptions.Value.ConnectionFactory));
        _compiler = moduleOptions.Value.Compiler 
            ?? throw new ArgumentException(nameof(moduleOptions.Value.Compiler));
    }

    /// <summary>
    /// Devuelve el primer elemento o el valor por
    /// defecto para un tipo que coincida con el query
    /// especificado
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> FirstOrDefault<T>(Query query)
    {
        try
        {
            // Commpilamos el query
            var sqlQuery = _compiler.Compile(query);
            // Armamos el comando
            var command = new CommandDefinition(commandText: sqlQuery.Sql, parameters: sqlQuery.NamedBindings, commandType: CommandType.Text);
            // Obtenemos una nueva conexion
            using IDbConnection connection = _connectionFactory();
            // Ejecutamos
            return await connection.QueryFirstOrDefaultAsync<T>(command);
        }
        catch (Exception ex)
        {
            return default;
        }
    }



    /// <summary>
    /// Devuelve una lista de los elementos que coinciden
    /// con el query especificado
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> Select<T>(Query query)
    {
        try
        {
            // Commpilamos el query
            var sqlQuery = _compiler.Compile(query);
            // Armamos el comando
            var command = new CommandDefinition(commandText: sqlQuery.Sql, parameters: sqlQuery.NamedBindings, commandType: CommandType.Text);
            // Obtenemos una nueva conexion
            using var connection = _connectionFactory();
            // Ejecutamos
            return await connection.QueryAsync<T>(command);
        }
        catch (Exception ex)
        {
            return default;
        }
    }

    /// <summary>
    /// Se encarga de ejecutar una consulta y paginarla segun la informacion
    /// de paginacion pasada por parametro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="pagedQuery"></param>
    /// <returns>Una lista de registros envuelta en informacion de paginacion</returns>
    public async Task<Paged<T>> Paginate<T>(Query query, PagedQuery pagedQuery)
    {
        await Task.CompletedTask;
        try
        {
            // Query para contar la cantidad de resultados
            var countQuery = query.Clone().AsCount();
            // Agregamos los detalles de paginacion y ordenamiento
            query.ForPage(pagedQuery.Page, pagedQuery.Results);
            // Compilamos ambos queries
            var countSql = _compiler.Compile(countQuery);
            var querySql = _compiler.Compile(query);
            // Creamos los commandos
            var countCommand = new CommandDefinition(commandText: countSql.Sql, 
                parameters: countSql.NamedBindings, 
                commandType: CommandType.Text);
            var queryCommand = new CommandDefinition(commandText: querySql.Sql,
                parameters: querySql.NamedBindings,
                commandType: CommandType.Text);
            // Creamos la conexion
            using var connection = _connectionFactory();
            // Ejecutamos cada consulta
            var quantity = await connection.QueryFirstOrDefaultAsync<int>(countCommand);
            var elements = await connection.QueryAsync<T>(queryCommand);
            var totalPages = (int)Math.Round((double)quantity / (double)pagedQuery.Results);// Math.Round(decimal(())
            // Creamos la respuesta paginada
            return new Paged<T>(elements.ToList().AsReadOnly(),pagedQuery.Page,pagedQuery.Results, totalPages, quantity);
        }
        catch (Exception ex)
        {
            return Paged<T>.AsEmpty;
        }
    }

}



/// <summary>
/// Opciones para usar los datos relacionales del modulo
/// </summary>
public class RelationalOptions<M>
    where M : IModule
{
    /// <summary>
    /// Indica el compilador para los datos relacionales
    /// </summary>
    public Compiler Compiler { get; set; }

    /// <summary>
    /// Indica la cadena de conexion para el modulo
    /// </summary>
    public Func<IDbConnection> ConnectionFactory { get; set; }
}
