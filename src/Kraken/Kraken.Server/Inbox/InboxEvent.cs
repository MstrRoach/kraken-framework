using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

public record InboxEvent(Type Event, Type Handler);
