using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

public record ReactionEvent(Type Event, Type Handler);
