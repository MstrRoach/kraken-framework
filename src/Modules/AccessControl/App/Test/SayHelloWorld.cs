using Kraken.Module.Request.Mediator;
using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

public class SayHelloWorldCommand : ICommand<HelloWorldSaid>
{
}

public class SayHelloWorldHandler : ComandHandlerBase<SayHelloWorldCommand, HelloWorldSaid>
{

    public override async Task<HelloWorldSaid> Handle(SayHelloWorldCommand request, CancellationToken cancellationToken)
    {
        var name = Context?.Identity?.Name;
        await Task.CompletedTask;
        return new HelloWorldSaid
        {
            Message = $"Hola {name ?? "desconocido"}"
        };
    }
}

public class HelloWorldSaid
{
    public string Message { get; set; }
}
