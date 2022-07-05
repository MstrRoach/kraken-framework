using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core;

/// <summary>
/// Marcador para las clases que son parte de una canalizacion
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PipelineStepAttribute : Attribute
{
}
