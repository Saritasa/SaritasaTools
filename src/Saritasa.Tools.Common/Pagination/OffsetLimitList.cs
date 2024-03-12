﻿// Copyright (c) 2015-2024, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Saritasa.Tools.Common.Pagination;

/// <summary>
/// List with limit and offset feature.
/// </summary>
/// <typeparam name="T">Source type.</typeparam>
/// <seealso cref="IEnumerable{T}" />
#if NET40 || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
[Serializable]
#endif
public class OffsetLimitList<T> : TotalCountList<T>
{
    /// <summary>
    /// The number of items to skip.
    /// </summary>
    public int Offset { get; protected internal set; }

    /// <summary>
    /// The maximum number of items to take.
    /// </summary>
    public int Limit { get; protected internal set; }

    /// <summary>
    /// Empty offset limit list.
    /// </summary>
    public static new OffsetLimitList<T> Empty { get; } =
        new OffsetLimitList<T>(new List<T>(), offset: 0, limit: 1, totalCount: 0);

    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    protected OffsetLimitList()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="items">Collection items.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <param name="limit">The maximum number of items to take.</param>
    /// <param name="totalCount">The total number of items in collection.</param>
    public OffsetLimitList(
        ICollection<T> items,
        int offset,
        int limit,
        int totalCount) : base(items, totalCount)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        this.Offset = offset;
        this.Limit = limit;
    }

#if NET40 || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Constructor for deserialization.
    /// </summary>
    /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
    /// <param name="context">Describes the source and destination of a given serialized stream,
    /// and provides an additional caller-defined context.</param>
    protected OffsetLimitList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
    }
#endif

    #region IMetadataEnumerable<OffsetLimitEnumerableMetadata, T>

    /// <summary>
    /// Get the offset limit metadata object.
    /// </summary>
    /// <returns>Offset limit metadata.</returns>
    private OffsetLimitListMetadata GetMetadata()
    {
        return new OffsetLimitListMetadata
        {
            Limit = Limit,
            Offset = Offset,
            TotalCount = TotalCount
        };
    }

    /// <summary>
    /// Create items object with metadata.
    /// </summary>
    /// <returns>List metadata.</returns>
    public new OffsetLimitMetadataDto<T> ToMetadataObject()
        => new OffsetLimitMetadataDto<T>(Items, GetMetadata());

    /// <summary>
    /// Convert items into another type.
    /// </summary>
    /// <param name="converter">Converter function.</param>
    /// <typeparam name="TNew">New type.</typeparam>
    /// <returns>New list.</returns>
    public new OffsetLimitList<TNew> Convert<TNew>(Func<T, TNew> converter)
        => new OffsetLimitList<TNew>
        {
            Items = Items.Select(converter).ToList(),
            Limit = Limit,
            Offset = Offset,
            TotalCount = TotalCount
        };

    #endregion
}
