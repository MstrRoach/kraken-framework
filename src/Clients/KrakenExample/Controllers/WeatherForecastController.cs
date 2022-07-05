using IdentityManagement;
using IdentityManagement.App.Account;
using Kraken.Core;
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
    private readonly IKrakenHost _host;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IKrakenHost krakenHost)
    {
        _logger = logger;
        _host = krakenHost;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        var command = new CreateAccountCommand
        {
            Email = "imct.jesus.antonio@gmail.com",
            Password = "adafsfsfsfsdfs"
        };
        var response = await _host.SendAsync(command);
        return StatusCode(StatusCodes.Status200OK,response);
    }
}
