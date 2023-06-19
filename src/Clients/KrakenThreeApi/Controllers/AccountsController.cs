using AccessControl.App.Accounts;
using Dottex.Kalypso.Server.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KrakenThreeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        readonly IAppHost _app;
        public AccountsController(IAppHost app)
        {
            _app = app;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var response = await _app.ExecuteAsync(command);
            return Ok(response);
        }

    }
}
