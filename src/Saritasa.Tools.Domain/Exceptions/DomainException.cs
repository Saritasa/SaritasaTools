﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
#if NET40
using System.Runtime.Serialization;
#endif

namespace Saritasa.Tools.Domain.Exceptions
{
    /// <summary>
    /// Exception occurs in domain part of application. It can be logic or validation exception.
    /// The message can be used as display messages to end user. InnerException should contain actual system exception.
    /// </summary>
#if NET40
    [Serializable]
#endif
    public class DomainException : Exception
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public DomainException() : base(DomainErrorDescriber.Default.Error())
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET40
        /// <summary>
        /// .ctor for deserialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream,
        /// and provides an additional caller-defined context.</param>
        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
