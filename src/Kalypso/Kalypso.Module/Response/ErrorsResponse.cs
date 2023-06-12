using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Response;

/// <summary>
/// Lista de errores 
/// </summary>
/// <param name="Errors"></param>
public record ErrorsResponse(params Error[] Errors);
