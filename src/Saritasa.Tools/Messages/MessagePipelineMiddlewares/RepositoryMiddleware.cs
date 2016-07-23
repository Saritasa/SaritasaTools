﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.CommandPipelineMiddlewares
{
    using System;

    /// <summary>
    /// Saves the command execution context to repository.
    /// </summary>
    public class RepositoryMiddleware : IMessagePipelineMiddleware
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "repository"; }
        }

        IMessageRepository repository;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="repository">Repository implementation.</param>
        public RepositoryMiddleware(IMessageRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            this.repository = repository;
        }

        /// <inheritdoc />
        public void Handle(Message message)
        {
            repository.Add(message);
        }
    }
}