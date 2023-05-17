using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Domain;

public interface ISpecification<T> where T : IAggregate
{
}
