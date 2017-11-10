﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Messages.Abstractions.Commands
{
    /// <summary>
    /// The class that contains commands handlers should be marked
    /// with this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CommandHandlersAttribute : Attribute
    {
    }
}
