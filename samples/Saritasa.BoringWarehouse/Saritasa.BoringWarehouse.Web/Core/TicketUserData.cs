﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using Saritasa.BoringWarehouse.Domain.Users.Entities;

namespace Saritasa.BoringWarehouse.Web.Core
{
    /// <summary>
    /// User ticket.
    /// </summary>
    public class TicketUserData
    {
        public int UserId { get; set; }

        public UserRole UserRole { get; set; }

        public override string ToString()
        {
            return string.Join(";", UserId.ToString(CultureInfo.InvariantCulture), UserRole);
        }

        public static TicketUserData FromString(string str)
        {
            var strarr = str.Split(';');
            return new TicketUserData
            {
                UserId = Convert.ToInt32(strarr[0]),
                UserRole = (UserRole)Enum.Parse(typeof(UserRole), strarr[1], true),
            };
        }

        public static TicketUserData FromContext(HttpContextBase context)
        {
            if (context != null && context.User.Identity.IsAuthenticated)
            {
                var formsIdentity = context.User.Identity as FormsIdentity;
                if (formsIdentity != null)
                {
                    return FromString(formsIdentity.Ticket.UserData);
                }
            }

            return new TicketUserData();
        }
    }
}
