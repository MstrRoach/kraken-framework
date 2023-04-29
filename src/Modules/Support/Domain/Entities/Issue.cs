using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Domain.Entities;

internal class Issue
{
    public Guid Id { get; set; }
    public Priority Priority { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CustomerComment { get; set; }
    public string SupportComment { get; set; }
    public Guid SupportId { get; set; }
    public string SupportName { get; set; }
}
