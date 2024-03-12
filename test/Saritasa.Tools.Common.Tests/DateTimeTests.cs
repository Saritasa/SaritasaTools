﻿// Copyright (c) 2015-2024, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Saritasa.Tools.Common.Utils;
#pragma warning disable CS1591

namespace Saritasa.Tools.Common.Tests;

/// <summary>
/// Date time tests.
/// </summary>
public class DateTimeTests
{
    public static readonly IEnumerable<object[]> GetStartOfPeriod_DateWithinMonth_StartOfMonth_Data = new[]
    {
        // Input, expected.
        new object[] { new DateTime(2017, 2, 3), new DateTime(2017, 2, 1) },
        new object[] { new DateTime(2016, 6, 3), new DateTime(2016, 6, 1) },
        new object[] { new DateTime(2017, 9, 30), new DateTime(2017, 9, 1) },
        new object[] { new DateTime(2017, 3, 1), new DateTime(2017, 3, 1) },
    };

    [Theory]
    [MemberData(nameof(GetStartOfPeriod_DateWithinMonth_StartOfMonth_Data))]
    public void GetStartOfPeriod_DateWithinMonth_StartOfMonth(DateTime input, DateTime beginOfMonth)
    {
        Assert.Equal(beginOfMonth, DateTimeUtils.GetStartOfPeriod(input, DateTimePeriod.Month));
    }

    public static readonly IEnumerable<object[]> GetEndOfPeriod_DateWithinMonth_EndOfMonth_Data = new[]
    {
        // Input, expected.
        new object[] { new DateTime(2017, 2, 3), new DateTime(2017, 2, 28) },
        new object[] { new DateTime(2016, 6, 3), new DateTime(2016, 6, 30) },
        new object[] { new DateTime(2017, 9, 30), new DateTime(2017, 9, 30) },
        new object[] { new DateTime(2017, 3, 1), new DateTime(2017, 3, 31) },
    };

    [Theory]
    [MemberData(nameof(GetEndOfPeriod_DateWithinMonth_EndOfMonth_Data))]
    public void GetEndOfPeriod_DateWithinMonth_EndOfMonth(DateTime input, DateTime endOfMonth)
    {
        Assert.Equal(endOfMonth, DateTimeUtils.GetEndOfPeriod(input, DateTimePeriod.Month).Date);
    }

    public static readonly IEnumerable<object[]> GetStartOfPeriod_DateWithinQuarter_StartOfQuarter_Data = new[]
    {
        // Input, expected.
        new object[] { new DateTime(2017, 2, 3), new DateTime(2017, 1, 1) },
        new object[] { new DateTime(2017, 5, 3), new DateTime(2017, 4, 1) },
        new object[] { new DateTime(2017, 7, 5), new DateTime(2017, 7, 1) },
        new object[] { new DateTime(2017, 11, 3), new DateTime(2017, 10, 1) },
    };

    [Theory]
    [MemberData(nameof(GetStartOfPeriod_DateWithinQuarter_StartOfQuarter_Data))]
    public void GetStartOfPeriod_DateWithinQuarter_StartOfQuarter(DateTime input, DateTime startOfQuarter)
    {
        Assert.Equal(startOfQuarter, DateTimeUtils.GetStartOfPeriod(input, DateTimePeriod.Quarter));
    }

    [Fact]
    public void GetDiff_ThreePairOfDates_26Or50()
    {
        // Arrange
        var diffa1 = new DateTime(2016, 10, 12);
        var diffa2 = new DateTime(2014, 8, 1);
        var diffb1 = new DateTime(2018, 10, 22);
        var diffb2 = new DateTime(2014, 8, 10);

        // Act & Assert
        Assert.InRange(DateTimeUtils.GetDiff(diffa1, diffa2, DateTimePeriod.Month), -26.355, -26.35);
        Assert.InRange(DateTimeUtils.GetDiff(diffa2, diffa1, DateTimePeriod.Month), 26.35, 26.355);
        Assert.InRange(DateTimeUtils.GetDiff(diffb2, diffb1, DateTimePeriod.Month), 50.38, 50.388);
    }

    [Fact]
    public void CombineDateAndTime_DifferentKind_ThrowArgumentException()
    {
        // Arrange
        var dt1 = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Local);
        var dt2 = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => { DateTimeUtils.CombineDateAndTime(dt1, dt2); });
    }

    [Fact]
    public void GetStartOfPeriod_USCulture_ReturnsSunday4()
    {
        // Arrange
        var dt = new DateTime(2017, 6, 5);

        // Act & Assert
        Assert.Equal(DayOfWeek.Sunday, DateTimeUtils.GetStartOfPeriod(dt, DateTimePeriod.Week, CultureInfo.InvariantCulture).DayOfWeek);
        Assert.Equal(4, DateTimeUtils.GetStartOfPeriod(dt, DateTimePeriod.Week, CultureInfo.InvariantCulture).Day);
    }

    [Fact]
    public void GetStartOfPeriod_RUCulture_ReturnsMonday5()
    {
        // Arrange
        var dt = new DateTime(2017, 6, 5);

        // Act & Assert
        Assert.Equal(DayOfWeek.Monday, DateTimeUtils.GetStartOfPeriod(dt, DateTimePeriod.Week, new CultureInfo("ru-RU")).DayOfWeek);
        Assert.Equal(5, DateTimeUtils.GetStartOfPeriod(dt, DateTimePeriod.Week, new CultureInfo("ru-RU")).Day);
    }

    [Fact]
    public void Truncate_USCulture_ReturnsJuly8()
    {
        // Arrange
        var date = new DateTime(2018, 7, 14, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = DateTimeUtils.Truncate(date, DateTimePeriod.Week, new CultureInfo("en-US"));

        // Assert
        Assert.Equal(8, result.Day);
    }

    [Fact]
    public void Truncate_RUCulture_ReturnsJuly9()
    {
        // Arrange
        var date = new DateTime(2018, 7, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = DateTimeUtils.Truncate(date, DateTimePeriod.Week, new CultureInfo("ru-RU"));

        // Assert
        Assert.Equal(9, result.Day);
    }

    [Fact]
    public void Truncate_CustomCulture_ReturnsJuly11()
    {
        // Arrange
        var date = new DateTime(2018, 7, 17, 0, 0, 0, DateTimeKind.Utc);
        var cultureInfo = new CultureInfo("us");
        cultureInfo.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Wednesday;

        // Act
        var result = DateTimeUtils.Truncate(date, DateTimePeriod.Week, cultureInfo);

        // Assert
        Assert.Equal(11, result.Day);
    }

    public static readonly IEnumerable<object[]> GetRange_DifferentPeriodDates_CorrectRange_Data = new[]
    {
        // Input, expected.
        new object[]
        {
            new DateTime(2020, 1, 1), new DateTime(2020, 1, 5), DateTimePeriod.Day,
            new DateTime[]
            {
                new DateTime(2020, 1, 1), new DateTime(2020, 1, 2), new DateTime(2020, 1, 3),
                new DateTime(2020, 1, 4), new DateTime(2020, 1, 5)
            }
        },
        new object[]
        {
            new DateTime(2020, 1, 1, 1, 1, 1), new DateTime(2020, 1, 1, 1, 1, 2), DateTimePeriod.Second,
            new DateTime[]
            {
                new DateTime(2020, 1, 1, 1, 1, 1), new DateTime(2020, 1, 1, 1, 1, 2)
            },
        },
        new object[]
        {
            new DateTime(2020, 3, 6), new DateTime(2020, 1, 1), DateTimePeriod.Month,
            new DateTime[]
            {
                new DateTime(2020, 3, 6), new DateTime(2020, 2, 6), new DateTime(2020, 1, 6)
            },
        }
    };

    [Theory]
    [MemberData(nameof(GetRange_DifferentPeriodDates_CorrectRange_Data))]
    public void GetRange_DifferentPeriodDates_CorrectRange(DateTime start, DateTime end, DateTimePeriod period, DateTime[] expected)
    {
        Assert.Equal(expected, DateTimeUtils.GetRange(start, end, period).ToArray());
    }
}
