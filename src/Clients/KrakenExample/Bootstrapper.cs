﻿using IdentityManagement;
using Kraken.Host;

namespace KrakenExample;

public static class Bootstrapper
{
    /// <summary>
    /// Configura el constructor del builder
    /// </summary>
    /// <returns></returns>
    public static Action<KrakenOptions> KrakenBuilder() => (builder) =>
    {
        builder.AddModule<IdentityModule>();
    };
}