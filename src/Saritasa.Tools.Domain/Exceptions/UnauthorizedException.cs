﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
#if NET40
using System.Runtime.Serialization;
#endif

namespace Saritasa.Tools.Domain.Exceptions
{
    /// <summary>
    /// Domain user is not unauthorized exception. Can be mapped to 401 HTTP status code.
    /// </summary>
#if NET40
    [Serializable]
#endif
    public class UnauthorizedException : DomainException
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public UnauthorizedException() : base(DomainErrorDescriber.Default.Unauthorized())
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public UnauthorizedException(string message) : base(message)
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET40
        /// <summary>
        /// .ctor for deserialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream,
        /// and provides an additional caller-defined context.</param>
        protected UnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
