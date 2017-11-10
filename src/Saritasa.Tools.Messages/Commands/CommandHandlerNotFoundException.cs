﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Messages.Commands
{
    /// <summary>
    /// Occurs when command handler cannot be found.
    /// </summary>
#if NET452
    [Serializable]
#endif
    public class CommandHandlerNotFoundException : Exception
    {
        /// <summary>
        /// .ctor
        /// </summary>
        private CommandHandlerNotFoundException()
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="commandName">Command name.</param>
        public CommandHandlerNotFoundException(string commandName) : base(
            string.Format(Properties.Strings.CommandHandlerNotFound, commandName))
        {
        }

#if NET452
        /// <summary>
        /// .ctor for deserialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream,
        /// and provides an additional caller-defined context.</param>
        protected CommandHandlerNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
