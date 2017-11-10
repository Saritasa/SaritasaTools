﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

using NameValueDict = System.Collections.Generic.IDictionary<string, object>;

namespace Saritasa.Tools.Emails
{
    /// <summary>
    /// This execution strategy does not do any operation itself. It just calls handler method.
    /// </summary>
    public class DefaultEmailExecutionStrategy : IEmailExecutionStrategy
    {
        /// <inheritdoc />
        public Task Execute(Func<MailMessage, NameValueDict, Task> handler, MailMessage message, NameValueDict data, CancellationToken cancellationToken)
        {
            return handler(message, data);
        }
    }
}
