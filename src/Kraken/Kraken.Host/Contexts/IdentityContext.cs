﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Kraken.Core.Contexts;

namespace Kraken.Host.Contexts;

/// <summary>
/// Pendiente modificar el contexto de identidad
/// </summary>
public class IdentityContext : IIdentityContext
{
    public bool IsAuthenticated { get; }
    public Guid Id { get; }
    public string Role { get; }

    public Dictionary<string, IEnumerable<string>> Claims { get; }

    private IdentityContext()
    {
    }

    public IdentityContext(Guid? id)
    {
        Id = id ?? Guid.Empty;
        IsAuthenticated = id.HasValue;
    }

    public IdentityContext(Guid? id, string role)
    {
        Id = id ?? Guid.Empty;
        IsAuthenticated = id.HasValue;
        Role = role;
    }

    public IdentityContext(ClaimsPrincipal principal)
    {
        if (principal?.Identity is null || string.IsNullOrWhiteSpace(principal.Identity.Name))
        {
            return;
        }

        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        Id = IsAuthenticated ? Guid.Parse(principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value) : Guid.Empty;
        Role = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        Claims = principal.Claims.GroupBy(x => x.Type)
            .ToDictionary(x => x.Key, x => x.Select(c => c.Value.ToString()));
    }

    public bool IsUser() => Role is "user";

    public bool IsAdmin() => Role is "admin";

    public static IIdentityContext Empty => new IdentityContext();
}