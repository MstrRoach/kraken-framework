﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dottex.Kalypso.Domain.Core
{
    /// <summary>
    /// Atributo oara ignorar un campo especifico
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
