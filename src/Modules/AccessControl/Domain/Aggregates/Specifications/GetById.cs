using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain.Aggregates.Specifications;

internal class GetById : MongoSpecification<Account>
{
    List<string> filters = new List<string>();
    public GetById(Guid id)
    {
        filters.Add(id.ToString());
    }

    public override List<string> Filters() 
        => filters;
}
