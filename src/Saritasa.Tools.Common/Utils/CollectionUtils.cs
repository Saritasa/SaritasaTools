﻿// Copyright(c) 2015-2022, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Saritasa.Tools.Common.Utils;

/// <summary>
/// Collection utils.
/// </summary>
public static class CollectionUtils
{
    private const int DefaultChunkSize = 1000;

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>An <see cref="System.Linq.IOrderedEnumerable{T}" /> whose elements are sorted according to a key.</returns>
    public static IOrderedEnumerable<TSource> Order<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        ListSortDirection sortOrder)
    {
        return sortOrder == ListSortDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order by using a specified identityComparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An System.Collections.Generic.IComparer to compare keys.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>An <see cref="System.Linq.IOrderedEnumerable{T}" /> whose elements are sorted according to a key.</returns>
    public static IOrderedEnumerable<TSource> Order<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IComparer<TKey> comparer,
        ListSortDirection sortOrder)
    {
        return sortOrder == ListSortDirection.Ascending ? source.OrderBy(keySelector, comparer) : source.OrderByDescending(keySelector, comparer);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="sortOrder">Sort order.</param>
    /// <returns>An <see cref="System.Linq.IOrderedQueryable{T}" /> whose elements are sorted according to a key.</returns>
    public static IOrderedQueryable<TSource> Order<TSource, TKey>(
        IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        ListSortDirection sortOrder)
    {
        return sortOrder == ListSortDirection.Ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

#if NETSTANDARD1_6_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Applies ordering to collection according to sorting entries and selectors. Allows to
    /// make ordering in one method call. For the first item, the OrderBy method call is applied.
    /// For subsequent items the ThenBy is used.
    /// </summary>
    /// <typeparam name="TSource">Type of data in the data source.</typeparam>
    /// <param name="source">Data source to order.</param>
    /// <param name="keySelectors">Contains mapping between sorting field name and the way to get this field from the object.</param>
    /// <param name="orderEntries">List of fields to sort by.</param>
    /// <returns>Reordered data.</returns>
    public static IOrderedEnumerable<TSource> OrderMultiple<TSource>(
        IEnumerable<TSource> source,
        ICollection<(string FieldName, ListSortDirection Order)> orderEntries,
        params (string FieldName, Func<TSource, object> Selector)[] keySelectors)
    {
        IEnumerable<Func<TSource, object>> GetKeySelectors(string fieldName)
        {
            var count = 0;
            for (int i = 0; i < keySelectors.Length; i++)
            {
                if (keySelectors[i].FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    yield return keySelectors[i].Selector;
                }
            }
            if (count == 0)
            {
                throw new InvalidOrderFieldException(fieldName, keySelectors.Select(k => k.FieldName).Distinct().ToArray());
            }
        }

        IOrderedEnumerable<TSource> OrderByField(IEnumerable<Func<TSource, object>> selectors,
            ListSortDirection direction,
            IEnumerable<TSource> target)
        {
            var orderedTarget = target as IOrderedEnumerable<TSource>;
            if (orderedTarget == null)
            {
                var firstSelector = selectors.FirstOrDefault();
                if (firstSelector != null)
                {
                    orderedTarget = direction == ListSortDirection.Ascending ?
                        target.OrderBy(firstSelector) :
                        target.OrderByDescending(firstSelector);
                }
                else
                {
                    orderedTarget = target.OrderBy(x => 1);
                }
                selectors = selectors.Skip(1);
            }

            foreach (var selector in selectors)
            {
                orderedTarget = direction == ListSortDirection.Ascending ?
                    orderedTarget.ThenBy(selector) :
                    orderedTarget.ThenByDescending(selector);
            }
            return orderedTarget;
        }

        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (orderEntries == null)
        {
            throw new ArgumentNullException(nameof(orderEntries));
        }
        if (keySelectors == null)
        {
            throw new ArgumentNullException(nameof(keySelectors));
        }

        // Sort for remaining fields.
        IEnumerable<TSource> ordered = source;
        foreach (var sortingEntry in orderEntries)
        {
            ordered = OrderByField(
                GetKeySelectors(sortingEntry.FieldName),
                sortingEntry.Order,
                ordered);
        }

        return ordered is IOrderedEnumerable<TSource> orderedResult ? orderedResult : ordered.OrderBy(s => 1);
    }

    /// <summary>
    /// Applies ordering to collection according to sorting entries and selectors. Allows to
    /// make ordering in one method call. For the first item, the OrderBy method call is applied.
    /// For subsequent items the ThenBy is used.
    /// </summary>
    /// <typeparam name="TSource">Type of data in the data source.</typeparam>
    /// <param name="source">Data source to order.</param>
    /// <param name="keySelectors">Contains mapping between sorting field name and the way to get this field from the object.</param>
    /// <param name="orderEntries">List of fields to sort by.</param>
    /// <returns>Reordered data.</returns>
    public static IOrderedQueryable<TSource> OrderMultiple<TSource>(
        IQueryable<TSource> source,
        ICollection<(string FieldName, ListSortDirection Order)> orderEntries,
        params (string FieldName, Expression<Func<TSource, object>> Selector)[] keySelectors)
    {
        IEnumerable<Expression<Func<TSource, object>>> GetKeySelectors(string fieldName)
        {
            var count = 0;
            for (int i = 0; i < keySelectors.Length; i++)
            {
                if (keySelectors[i].FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    yield return keySelectors[i].Selector;
                }
            }
            if (count == 0)
            {
                throw new InvalidOrderFieldException(fieldName, keySelectors.Select(k => k.FieldName).Distinct().ToArray());
            }
        }

        bool useFirstOrderBy = true;

        IOrderedQueryable<TSource> OrderByField(IEnumerable<Expression<Func<TSource, object>>> selectors,
            ListSortDirection direction,
            IQueryable<TSource> target)
        {
            if (useFirstOrderBy)
            {
                var firstSelector = selectors.FirstOrDefault();
                if (firstSelector != null)
                {
                    target = direction == ListSortDirection.Ascending ?
                    target.OrderBy(firstSelector) :
                    target.OrderByDescending(firstSelector);
                    selectors = selectors.Skip(1);
                    useFirstOrderBy = false;
                }
            }

            var orderedTarget = target as IOrderedQueryable<TSource>;
            if (orderedTarget == null)
            {
                throw new InvalidOperationException(Properties.Strings.ArgumentMustBeOrderable);
            }
            foreach (var selector in selectors)
            {
                orderedTarget = direction == ListSortDirection.Ascending ?
                    orderedTarget.ThenBy(selector) :
                    orderedTarget.ThenByDescending(selector);
            }
            return orderedTarget;
        }

        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (orderEntries == null)
        {
            throw new ArgumentNullException(nameof(orderEntries));
        }
        if (keySelectors == null)
        {
            throw new ArgumentNullException(nameof(keySelectors));
        }

        // Sort for remaining fields.
        IQueryable<TSource> ordered = source;
        foreach (var sortingEntry in orderEntries)
        {
            ordered = OrderByField(
                GetKeySelectors(sortingEntry.FieldName),
                sortingEntry.Order,
                ordered);
        }

        return ordered is IOrderedQueryable<TSource> orderedResult ? orderedResult : ordered.OrderBy(s => 1);
    }
#endif

    /// <summary>
    /// Breaks a list of items into chunks of a specific size. Be aware that this method generates one additional
    /// query to get the total number of collection elements.
    /// </summary>
    /// <param name="source">Source list.</param>
    /// <param name="chunkSize">Chunk size.</param>
    /// <returns>Enumeration of queryable collections.</returns>
    public static IEnumerable<IQueryable<T>> ChunkSelectRange<T>(
        IQueryable<T> source,
        int chunkSize = DefaultChunkSize)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (chunkSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize),
                string.Format(Properties.Strings.ArgumentMustBeGreaterThan, nameof(chunkSize), 1));
        }

        var totalNumberOfElements = source.Count();
        int currentPosition = 0;
        var originalSource = source;
        while (totalNumberOfElements > currentPosition)
        {
            yield return originalSource.Take(chunkSize);
            originalSource = originalSource.Skip(chunkSize);
            currentPosition += chunkSize;
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Breaks a list of async items into chunks of a specific size.
    /// </summary>
    /// <param name="source">Source list.</param>
    /// <param name="chunkSize">Chunk size.</param>
    /// <param name="skipCount">Number of items to skip.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>Enumeration of enumerable collections.</returns>
    public static async IAsyncEnumerable<IAsyncEnumerable<T>> ChunkSelectRangeAsync<T>(
        IAsyncEnumerable<T> source,
        int chunkSize = DefaultChunkSize,
        int skipCount = 0,
        [System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (chunkSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize),
                string.Format(Properties.Strings.ArgumentMustBeGreaterThan, nameof(chunkSize), 1));
        }
        if (skipCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skipCount),
                string.Format(Properties.Strings.ArgumentMustBeGreaterThan, nameof(skipCount), 0));
        }

        var enumerator = source.GetAsyncEnumerator(cancellationToken);

        async IAsyncEnumerable<T> Batch()
        {
            yield return enumerator.Current;
            for (int i = 1; i < chunkSize; i++)
            {
                if (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    yield return enumerator.Current;
                }
            }
        }

        for (var i = 0; i < skipCount; i++)
        {
            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield break;
            }
        }

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return Batch();
        }
    }
#endif

    /// <summary>
    /// Breaks a list of items into chunks of a specific size.
    /// </summary>
    /// <param name="source">Source list.</param>
    /// <param name="chunkSize">Chunk size.</param>
    /// <returns>Enumeration of enumerable collections.</returns>
    public static IEnumerable<IEnumerable<T>> ChunkSelectRange<T>(
        IEnumerable<T> source,
        int chunkSize = DefaultChunkSize)
    {
#if NET6_0_OR_GREATER
        return source.Chunk<T>(chunkSize);
#else
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (chunkSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize),
                string.Format(Properties.Strings.ArgumentMustBeGreaterThan, nameof(chunkSize), 1));
        }

        long totalNumberOfElements = source.Count();
        int currentPosition = 0;
        var originalSource = source;
        while (totalNumberOfElements > currentPosition)
        {
            yield return originalSource.Take(chunkSize);
            originalSource = originalSource.Skip(chunkSize);
            currentPosition += chunkSize;
        }
#endif
    }

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified <see cref="IEqualityComparer{T}" />
    /// to compare values and specified key selector to get a distinct key. This method is implemented by
    /// using deferred execution.
    /// </summary>
    /// <typeparam name="TSource">Type of the source sequence.</typeparam>
    /// <typeparam name="TKey">Type of the projected element.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">Projection for determining "distinctness".</param>
    /// <param name="comparer">The equality identityComparer to compare key values. If null default identityComparer will be used.</param>
    /// <returns>A collection that contains distinct elements from the source sequence.</returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
    {
#if NET6_0_OR_GREATER
        return source.DistinctBy(keySelector, comparer);
#else
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        IEnumerable<TSource> DistinctByImpl()
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        return DistinctByImpl();
#endif
    }

    /// <summary>
    /// Compares two collections and creates a diff with items to add, remove and update based
    /// on identity equality.
    ///
    /// The updated items are formatted using comparer argument. If defined, it is used to determine whether
    /// to add items to "updated" collection. If not the method tries to use Equals call.
    /// </summary>
    /// <param name="source">Source collection.</param>
    /// <param name="target">Target collection (new).</param>
    /// <param name="identityComparer">Comparer elements by identity. This means that elements might be
    /// identity equal but target collection has its newer version.</param>
    /// <param name="dataComparer">Comparer for full objects compare. If not defined every object considered as updated.</param>
    /// <typeparam name="T">Item type.</typeparam>
    /// <returns>Items to add, remove and update.</returns>
    public static DiffResult<T> Diff<T>(
        IEnumerable<T> source,
        IEnumerable<T> target,
        Func<T, T, bool> identityComparer,
        Func<T, T, bool>? dataComparer = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }
        if (identityComparer == null)
        {
            throw new ArgumentNullException(nameof(identityComparer));
        }
        dataComparer ??= (o1, o2) => o1 != null && o1.Equals(o2);

        var sourceArr = source is ICollection<T> sourceList ? sourceList : source.ToArray();
        var targetArr = target is IList<T> targetList ? targetList : target.ToArray();
        var added = new List<T>(targetArr.Count / 2);
        var updated = new List<DiffResultUpdatedItems<T>>(sourceArr.Count);
        var removed = new List<T>(sourceArr.Count / 2);
        var targetBits = new BitArray(targetArr.Count);

        // Determine updated and removed elements.
        foreach (var sourceItem in sourceArr)
        {
            bool isRemoved = true;
            for (int j = 0; j < targetArr.Count; j++)
            {
                if (targetBits[j])
                {
                    continue;
                }

                // Elements are identity equal.
                if (identityComparer(sourceItem, targetArr[j]))
                {
                    if (!dataComparer(sourceItem, targetArr[j]))
                    {
                        updated.Add(new DiffResultUpdatedItems<T>(sourceItem, targetArr[j]));
                    }
                    targetBits[j] = true;
                    isRemoved = false;
                    break;
                }
            }

            if (isRemoved)
            {
                removed.Add(sourceItem);
            }
        }

        // Determine new elements.
        added.AddRange(targetArr.Where((t, i) => !targetBits[i]));

        // Format result.
        return new DiffResult<T>(added, removed, updated);
    }

    /// <summary>
    /// Compares two collections and creates a diff with items to add, remove and update based
    /// on identity equality.
    ///
    /// The updated items are formatted using comparer argument. If defined, it is used to determine whether
    /// to add items to "updated" collection. If not the method tries to use Equals call.
    /// </summary>
    /// <param name="source">Source collection.</param>
    /// <param name="target">Target collection (new).</param>
    /// <param name="identityEqualityComparer">Comparer elements by identity. This means that elements might be
    /// identity equal but target collection has its newer version. If null the Equals method will be used.</param>
    /// <param name="dataComparer">Comparer for full objects compare. If not defined every object considered as updated.</param>
    /// <typeparam name="T">Item type.</typeparam>
    /// <returns>Items to add, remove and update.</returns>
    public static DiffResult<T> Diff<T>(
        IEnumerable<T> source,
        IEnumerable<T> target,
        IEqualityComparer<T>? identityEqualityComparer = null,
        Func<T, T, bool>? dataComparer = null
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }
        identityEqualityComparer ??= EqualityComparer<T>.Default;
        dataComparer ??= (o1, o2) => o1 != null && o1.Equals(o2);

        var sourceArr = source is ICollection<T> sourceList ? sourceList : source.ToArray();
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        var targetSet = new HashSet<T>(target, identityEqualityComparer);
#else
        var targetSet = target.ToDictionary(t => t, t => t, identityEqualityComparer);
#endif
        var added = new List<T>(targetSet.Count / 2);
        var updated = new List<DiffResultUpdatedItems<T>>(sourceArr.Count / 2);
        var removed = new List<T>(sourceArr.Count / 2);

        foreach (var sourceItem in sourceArr)
        {
            if (targetSet.TryGetValue(sourceItem, out T? targetItemToCompare))
            {
                if (!dataComparer(sourceItem, targetItemToCompare))
                {
                    // No data equal - for update.
                    updated.Add(new DiffResultUpdatedItems<T>(sourceItem, targetItemToCompare));
                }
                targetSet.Remove(targetItemToCompare);
            }
            else
            {
                // Cannot find - to remove.
                removed.Add(sourceItem);
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        added.AddRange(target.Where(item => targetSet.Contains(item)));
#else
        added.AddRange(target.Where(item => targetSet.ContainsKey(item)));
#endif

        return new DiffResult<T>(added, removed, updated);
    }

    /// <summary>
    /// Apply collections compare diff to another collection.
    /// </summary>
    /// <param name="source">The collection to compare to.</param>
    /// <param name="diff">Diff.</param>
    /// <param name="update">Optional updater delegate to change state of object 1 according to object 2 and return
    /// destination object. The new object is not used and this overload is only for AutoMapper compatibility.</param>
    /// <typeparam name="T">Collection source type.</typeparam>
    public static void ApplyDiff<T>(ICollection<T> source, DiffResult<T> diff, Func<T, T, T>? update = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (diff == null)
        {
            throw new ArgumentNullException(nameof(diff));
        }

        foreach (T removedItem in diff.Removed)
        {
            source.Remove(removedItem);
        }
        foreach (T addedItem in diff.Added)
        {
            source.Add(addedItem);
        }

        if (update != null)
        {
            foreach ((T sourceItem, T targetItem) in diff.Updated)
            {
                update.Invoke(sourceItem, targetItem);
            }
        }
    }

    /// <summary>
    /// Apply collections compare diff to another collection.
    /// </summary>
    /// <param name="source">The collection to compare to.</param>
    /// <param name="diff">Diff.</param>
    /// <param name="update">Optional updater delegate to change state of object 1 according to object 2.</param>
    /// <typeparam name="T">Collection source type.</typeparam>
    public static void ApplyDiff<T>(ICollection<T> source, DiffResult<T> diff, Action<T, T>? update = null)
    {
        ApplyDiff(source, diff, (obj1, obj2) =>
        {
            update?.Invoke(obj1, obj2);
            return obj2;
        });
    }

    /// <summary>
    /// Iterate over a collection handling 2 items at a time.
    /// </summary>
    /// <typeparam name="T">Item source type.</typeparam>
    /// <param name="source">Source collection.</param>
    /// <returns>Enumeration in pairs.</returns>
    /// <example>
    /// Array [1, 2, 3, 4] will be transformed to [(1, 2), (2, 3), (3, 4)].
    /// </example>
    public static IEnumerable<(T Item1, T Item2)> Pairwise<T>(IEnumerable<T> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        var previous = default(T);
        using (var iterator = source.GetEnumerator())
        {
            if (iterator.MoveNext())
            {
                previous = iterator.Current;
            }

            while (iterator.MoveNext())
            {
                yield return (previous!, iterator.Current);
                previous = iterator.Current;
            }
        }
    }
}
