using IdentityManagement;
using IdentityManagement.App.AccountManagement;
using Kraken.Core;
using Kraken.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KrakenExample.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    private readonly ILogger<AccountController> _logger;
    private readonly IAppHost _apphost;

    public AccountController(ILogger<AccountController> logger, IAppHost apphost)
    {
        _logger = logger;
        _apphost = apphost;
    }

    //[Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        //var command = new CreateAccountCommand
        //{
        //    Email = "imct.jesus.antonio@gmail.com",
        //    Password = "adafsfsfsfsdfs"
        //};
        //var response = await _apphost.SendAsync(command);
        return StatusCode(StatusCodes.Status200OK, Summaries);
    }
}

public class UnsupportedException : KrakenException
{
    public UnsupportedException(string message) : 
        base(message)
    {
    }
}
