using Dottex.Kalypso.Domain.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain.Aggregates.ProfileAggregate;

internal class ProfileSpecification
{

    public static ISpecification<Profile> GetAll()
        => GenericSpecification<Profile>.Create(x => 1 == 1);
}
