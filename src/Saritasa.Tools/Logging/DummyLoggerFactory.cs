﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Logging
{
    /// <summary>
    /// Dummy logger factory that returns dummy logger.
    /// </summary>
    public class DummyLoggerFactory : ILoggerFactory
    {
        /// <inheritdoc />
        public ILogger GetLogger(string name)
        {
            return new DummyLogger();
        }
    }
}