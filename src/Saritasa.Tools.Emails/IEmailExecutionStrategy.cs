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
    /// Email execution strategies.
    /// </summary>
    public interface IEmailExecutionStrategy
    {
        /// <summary>
        /// Execute mail send handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="message">Mail message.</param>
        /// <param name="data">Additional data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task Execute(
            Func<MailMessage, NameValueDict, Task> handler,
            MailMessage message, NameValueDict data,
            CancellationToken cancellationToken);
    }
}
