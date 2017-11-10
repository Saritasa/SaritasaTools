﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;

namespace Saritasa.Tools.Emails.Interceptors
{
    /// <summary>
    /// Contains counters of sending and sent emails. Thread safe.
    /// </summary>
    public class CountEmailsInterceptor : IEmailInterceptor
    {
        private int sendingCallCount, sentCallCount;

        /// <summary>
        /// Sending messages count. It is not necessary that all of these
        /// messages were actually sent.
        /// </summary>
        public int SendingCallCount => sendingCallCount;

        /// <summary>
        /// Sent messages count.
        /// </summary>
        public int SentCallCount => sentCallCount;

        #region IEmailInterceptor implementation

        /// <inheritdoc />
        public void Sending(MailMessage mailMessage, IDictionary<string, object> data, ref bool cancel)
        {
            Interlocked.Increment(ref sendingCallCount);
        }

        /// <inheritdoc />
        public void Sent(MailMessage mailMessage, IDictionary<string, object> data)
        {
            Interlocked.Increment(ref sentCallCount);
        }

        #endregion

        /// <summary>
        /// Reset counters.
        /// </summary>
        public void Reset()
        {
            sendingCallCount = 0;
            sentCallCount = 0;
        }
    }
}
