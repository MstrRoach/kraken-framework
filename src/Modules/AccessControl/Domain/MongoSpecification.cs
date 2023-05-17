using Kraken.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain;

internal abstract class MongoSpecification<T> : ISpecification<T>
    where T : IAggregate
{
    public abstract List<String> Filters();
}
