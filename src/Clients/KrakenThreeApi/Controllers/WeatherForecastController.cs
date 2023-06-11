using AccessControl.App.Test;
using AccessControl.Domain.Aggregates.AccountAggregate;
using Kraken.Server.Request;
using Microsoft.AspNetCore.Mvc;

namespace KrakenThreeApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IAppHost _appHost;
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IAppHost appHost)
    {
        _logger = logger;
        _appHost = appHost;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await _appHost.ExecuteAsync(new SayHelloWorldCommand());
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost("Account")]
    public async Task<IActionResult> CreateAccountAsync()
    {
        var response = await _appHost.ExecuteAsync(new SayHelloWorldCommand());
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("Account")]
    public async Task<IActionResult> GetAllAccount()
    {
        var response = await _appHost.ReadAsync(new GetAllAccounts());
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpPut("Account")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountCommand command)
    {
        var response = await _appHost.ExecuteAsync(command);
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpDelete("Account")]
    public async Task<IActionResult> DeleteAccount([FromQuery]Guid id)
    {
        var command = new DeleteAccountCommand { Id = id };
        var response = await _appHost.ExecuteAsync(command);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}