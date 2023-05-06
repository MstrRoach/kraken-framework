using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Response;

/// <summary>
/// Lista de errores 
/// </summary>
/// <param name="Errors"></param>
public record Errors(params Error[] Errors);
