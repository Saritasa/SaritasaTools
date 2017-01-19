﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Domain.Exceptions
{
    using System;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Domain conflict exception. Indicates that the request could not be
    /// processed because of conflict in the request, such as an edit conflict between multiple simultaneous updates.
    /// Can be mapped to 409 HTTP status code.
    /// </summary>
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
    [Serializable]
#endif
    public class ConflictException : DomainException
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public ConflictException() : base("Conflict occurred")
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public ConflictException(string message) : base(message)
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
        /// <summary>
        /// .ctor for deserialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream,
        /// and provides an additional caller-defined context.</param>
        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}