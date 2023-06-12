using Dottex.Kalypso.Module.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server;

internal class ServerBootstrapper : BackgroundService
{
    /// <summary>
    /// Logger del arrancador
    /// </summary>
    private readonly ILogger<ServerBootstrapper> _logger;

    /// <summary>
    /// Proveedor de servicios
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Lista de procesadores ejecutables
    /// </summary>
    private IEnumerable<IProcessingService> _services = default;

    /// <summary>
    /// Generador de token de cancelacion
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Bandera para indicar si todo el componente fue liberado
    /// </summary>
    private bool _disposed;

    public ServerBootstrapper(ILogger<ServerBootstrapper> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Ejecuta el trabajo en segundo plano del arrancador
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Run();

    /// <summary>
    /// Ejecuta las operaciones del arrancador
    /// </summary>
    /// <returns></returns>
    public async Task Run()
    {
        _logger.LogInformation("<<<<<<<<<< Kalypso background task is starting.>>>>>>>>>>");
        try
        {
            _logger.LogInformation("=========> Loading processors to execute . . .");
            // Cargamos los procesadores
            _services = _serviceProvider.GetServices<IProcessingService>();
        }
        catch (Exception ex)
        {
            _logger.LogInformation("xxxxxxxxxx Error to load processors. xxxxxxxxxx");
        }
        // Registramos el comportamiento de cancelacion
        _cts.Token.Register(() =>
        {
            _logger.LogInformation("<========= Kalypso background task is stopping.");
            foreach (var item in _services)
            {
                try
                {
                    item.Dispose();
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogWarning(ex, $"Expected an OperationCanceledException, but found '{ex.Message}'.");
                }
            }
        });

        // Iniciamos el proceso central
        await Process();
        // Ponemos el log de debug
        _logger.LogInformation(kalypso_logo);
    }

    /// <summary>
    /// Processammiento principal para iniciar todos los servicios
    /// </summary>
    /// <returns></returns>
    protected virtual Task Process()
    {
        // Iniciamos el proceso en cada servicio
        foreach (var item in _services)
        {
            // Lanzamos error si se solicita la cancelarcion
            _cts.Token.ThrowIfCancellationRequested();

            try
            {
                _logger.LogInformation("==========> Starting processin service {service}", item.GetType().Name);
                // Iniciando el proceso
                item.Start(_cts.Token);
                _logger.LogInformation("==========> Process {service} is started", item.GetType().Name);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Starting the processors throw an exception.");
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Operacion realizada cuando el trabajo en segundo plano se detiene
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        // Indicamos con el token de cancelacion que se cancele
        _cts.Cancel();
        // Detenemos el proceso interno
        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Libera los recursos administrados por
    /// el servicio en segundo plano
    /// </summary>
    public override void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _cts.Cancel();
        _cts.Dispose();
        _disposed = true;
    }


    private const string kalypso_logo = @"

                                    Kalypso - Powered by Dottex

                                             .~7JYYJ7~.                                             
                                           ~5B########BP~                                           
                                         .5##############5.                                         
                                        .G################G.                                        
                                 ..     J##################J     ..                                 
                             .!5GBBGY:  G##################G  :YGBBG5!.                             
                            7B&BJ!~!P#7 G##################G 7#P!~!JB&B7                            
                           7##P.:?  .#G Y##################Y G#.  ?:.P##7                           
                          .G##! :5?75P^ :B################B: ^P57?5: !##B.                          
                           P##P. .:^:    ^B##############B^    :^:  .P##P                           
                           ^###P^         Y##############Y         ^P###^                           
                            :P###GJ!:..   .P############P.   ..:!JG###P:                            
                        ^7Y5J??P##&&#BBGP55B############B55PGGB##&&#P??JYY7^                        
                      .P#G7^~JB!.!YG############################G5!.!BJ~^7G#P.                      
                      J&#:.^.:BJ   ..^J######################J^..   JB:.^.:#&J                      
                      !##Y.:~!^    .:!P######################P!:.    ^!~:.Y##7                      
                       ?#&BJ~^^~!?5G#&###BPP############PPB###&#G5?!~^^~JB&#?                       
                        :JG#&&&&&&&#BP57^.  ~####PP####!  .^7YPB#&&&&&&&#GJ:                        
                           :^!777!^^?PP55?. .B###77###B. .?55PP?^^!777!^:.                          
                                  .P#J...PY :####::####: YP...J#P.                                  
                                  ^&#:  .?: 5###J  J###5 :?.  :#&^                                  
                                  .5&B7:^^!G###?    ?###G!^^:7B&5.                                  
                                    !P######G?:      :?G######P!                                    
                                      .:^^:.            .:^^:.  

        Features:
            - Cqrs processing
            - Transaccional support
            - Outbox pattern event
            - Handler to events
            - Preconfigured server
            - Support for modular monoliths and microservices
";
}
