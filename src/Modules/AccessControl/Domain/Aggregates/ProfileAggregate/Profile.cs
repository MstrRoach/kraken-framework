using Kraken.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain.Aggregates.ProfileAggregate;

public class Profile : Aggregate<int>
{
    /// <summary>
    /// Nombre del perfil
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Fecha de nacimiento
    /// </summary>
    public DateTime Birthdate { get; private set; }

    /// <summary>
    /// Creacion del perfil
    /// </summary>
    /// <param name="name"></param>
    /// <param name="birthdate"></param>
    /// <returns></returns>
    public static Profile Create(string name, DateTime birthdate)
    {
        var profile = new Profile
        {
            Id = 0,
            Name = name,
            Birthdate = birthdate
        };
        return profile;
    }

    private Profile() { }
}
