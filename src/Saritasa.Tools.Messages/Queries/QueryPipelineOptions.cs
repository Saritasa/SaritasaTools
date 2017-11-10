﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using Saritasa.Tools.Messages.Common;

namespace Saritasa.Tools.Messages.Queries
{
    /// <summary>
    /// Query pipeline options.
    /// </summary>
    public class QueryPipelineOptions : MessagePipelineOptions
    {
        /// <summary>
        /// Include execution duration.
        /// </summary>
        public bool IncludeExecutionDuration { get; set; } = true;
    }
}
