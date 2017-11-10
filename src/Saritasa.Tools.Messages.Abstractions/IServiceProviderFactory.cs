﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Messages.Abstractions
{
    /// <summary>
    /// Service provider factory.
    /// </summary>
    public interface IServiceProviderFactory
    {
        /// <summary>
        /// Creates service provider.
        /// </summary>
        /// <returns>Service provider.</returns>
        IServiceProvider Create();
    }
}
