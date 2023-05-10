using AccessControl.App.Test;
using Kraken.Server.Request;
using Microsoft.AspNetCore.Mvc;

namespace KrakenThreeApi.Controllers
{
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
    }
}