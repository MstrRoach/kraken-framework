using System;

namespace Kraken.Core.Exceptions;

public abstract class KrakenException : Exception
{
    protected KrakenException(string message) : base(message)
    {
    }
}