﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Emails
{
    using System.Collections.Generic;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
    using System.Net.Mail;
#endif

    /// <summary>
    /// Email interceptor to be used with EmailSender.
    /// </summary>
    public interface IEmailInterceptor
    {
        /// <summary>
        /// The method is called after success email sending.
        /// </summary>
        /// <param name="mailMessage">Mail message.</param>
        /// <param name="data">Additional data.</param>
        void Sent(MailMessage mailMessage, IDictionary<string, object> data);

        /// <summary>
        /// The method is called before email sending.
        /// </summary>
        /// <param name="mailMessage">Mail message.</param>
        /// <param name="data">Additional data.</param>
        /// <param name="cancel">Should the email sending be cancelled.</param>
        void Sending(MailMessage mailMessage, IDictionary<string, object> data, ref bool cancel);
    }
}
