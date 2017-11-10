﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using Xunit;
using Saritasa.Tools.Common.Utils;

namespace Saritasa.Tools.Common.Tests
{
    /// <summary>
    /// Validation module test.
    /// </summary>
    public class GuardTests
    {
        [Fact]
        public void Valid_emails_should_not_throw_exception()
        {
            Guard.IsNotInvalidEmail("fwd2ivan@yandex.ru", "test");
            Guard.IsNotInvalidEmail("fwd2ivan+label@yandex.ru", "test");
            Guard.IsNotInvalidEmail("2fwd2ivan@yandex.ru", "test");
            Guard.IsNotInvalidEmail("ivan+ivan@kras.saritas.local", "test");
        }

        [Fact]
        public void Invalid_emails_should_throw_exception()
        {
            Assert.Throws<ArgumentException>(() => { Guard.IsNotInvalidEmail("fwd2ivanyandex.ru", "test"); });
            Assert.Throws<ArgumentException>(() => { Guard.IsNotInvalidEmail("2fwd2ivan@yandex", "test"); });
            Assert.Throws<ArgumentException>(() => { Guard.IsNotInvalidEmail("@yandex.ru", "test"); });
            Assert.Throws<ArgumentException>(() => { Guard.IsNotInvalidEmail("fwd2ivan@", "test"); });
        }

        [Fact]
        public void Is_not_null_should_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Guard.IsNotNull(null, "obj");
            });
        }
    }
}
