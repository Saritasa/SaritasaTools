﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Messages.Abstractions.Commands
{
    /// <summary>
    /// Marks property as output. Not for processing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandOutAttribute : Attribute
    {
    }
}
