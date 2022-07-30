using IdentityManagement;
using IdentityManagement.App.AccountManagement;
using Kraken.Core;
using Kraken.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KrakenExample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IAppHost _apphost;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IAppHost apphost)
    {
        _logger = logger;
        _apphost = apphost;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        throw new UnsupportedException("Error al ejecutar la consulta");

        var command = new CreateAccountCommand
        {
            Email = "imct.jesus.antonio@gmail.com",
            Password = "adafsfsfsfsdfs"
        };
        var response = await _apphost.SendAsync(command);
        return StatusCode(StatusCodes.Status200OK,response);
    }
}

public class UnsupportedException : KrakenException
{
    public UnsupportedException(string message) : 
        base(message)
    {
    }
}
