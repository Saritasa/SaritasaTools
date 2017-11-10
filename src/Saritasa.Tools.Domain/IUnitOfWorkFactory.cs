﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Domain
{
    /// <summary>
    /// Unit of work factory abstraction.
    /// </summary>
    public interface IUnitOfWorkFactory<out TUnitOfWork> where TUnitOfWork : class
    {
        /// <summary>
        /// Creates unit of work with default isolation level.
        /// </summary>
        /// <returns>Unit of work.</returns>
        TUnitOfWork Create();
    }
}
