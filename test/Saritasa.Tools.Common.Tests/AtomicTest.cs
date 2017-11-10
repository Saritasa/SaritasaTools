﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using Xunit;
using Saritasa.Tools.Common.Utils;

namespace Saritasa.Tools.Common.Tests
{
    /// <summary>
    /// Atomic tests.
    /// </summary>
    public class AtomicTest
    {
        [Fact]
        public void Cas_should_process()
        {
            // Arrange
            int a = 5;

            // Act
            AtomicUtils.DoWithCas(ref a, v => v * 15);

            // Assert
            Assert.Equal(75, a);
        }

        [Fact]
        public void Atomic_swap_should_swap_values()
        {
            // Arrange
            int a = 2, b = 5;

            // Act
            AtomicUtils.Swap(ref a, ref b);

            // Assert
            Assert.Equal(5, a);
            Assert.Equal(2, b);
        }
    }
}
