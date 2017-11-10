﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Saritasa.Tools.Messages.Abstractions;
using Saritasa.Tools.Messages.Abstractions.Queries;

namespace Saritasa.Tools.Messages.Queries
{
    /// <summary>
    /// Query pipeline extensions.
    /// </summary>
    public static class MessagePipelinesContainerExtensions
    {
        /// <summary>
        /// Add query pipeline feature to message context.
        /// </summary>
        /// <param name="messagePipelineContainer">Pipeline container.</param>
        /// <returns>Query pipeline builder.</returns>
        public static QueryPipelineBuilder AddQueryPipeline(this IMessagePipelineContainer messagePipelineContainer)
        {
            return AddQueryPipeline(messagePipelineContainer, options => { });
        }

        /// <summary>
        /// Add query pipeline feature to message context.
        /// </summary>
        /// <param name="messagePipelineContainer">Pipeline container.</param>
        /// <param name="setupAction">Action to setup query pipeline.</param>
        /// <returns>Query pipeline builder.</returns>
        public static QueryPipelineBuilder AddQueryPipeline(this IMessagePipelineContainer messagePipelineContainer,
            Action<QueryPipelineOptions> setupAction)
        {
            if (messagePipelineContainer.Pipelines.Any(p => p is IQueryPipeline))
            {
                throw new InvalidOperationException("Queries pipeline already exists in global context items. " +
                                                    "Use RemovePipeline method to clean up existins pipeline.");
            }

            var queryPipeline = new QueryPipeline();
            setupAction(queryPipeline.Options);
            var list = messagePipelineContainer.Pipelines.ToList();
            list.Add(queryPipeline);
            messagePipelineContainer.Pipelines = list.ToArray();

            return new QueryPipelineBuilder(queryPipeline);
        }
    }
}
