// Copyright (c) 2015-2024, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;

namespace Saritasa.Tools.Common.Utils;

/// <summary>
/// The struct represents the pair of source item in collection and the same (by identity)
/// item in target collection.
/// </summary>
/// <typeparam name="T">The source type.</typeparam>
#if NET40 || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
[Serializable]
#endif
public readonly struct DiffResultUpdatedItems<T>
{
    /// <summary>
    /// The source item.
    /// </summary>
    public T Source { get; }

    /// <summary>
    /// The target item.
    /// </summary>
    public T Target { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="source">The source item.</param>
    /// <param name="target">The target item.</param>
    public DiffResultUpdatedItems(T source, T target)
    {
        this.Source = source ?? throw new ArgumentNullException(nameof(source));
        this.Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    /// <summary>
    /// Deconstruction method used for tuples.
    /// </summary>
    /// <param name="source">The source item.</param>
    /// <param name="target">The target item.</param>
    public void Deconstruct(out T source, out T target)
    {
        source = this.Source;
        target = this.Target;
    }

#if NETSTANDARD1_6_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Allows to implicitly convert to the type from tuple.
    /// </summary>
    /// <param name="tuple">The tuple to convert from.</param>
    /// <returns>The <see cref="DiffResultUpdatedItems{T}" /> instance.</returns>
    public static implicit operator DiffResultUpdatedItems<T>((T Item1, T Item2) tuple)
        => new DiffResultUpdatedItems<T>(tuple.Item1, tuple.Item2);
#endif

    #region IEquatable<ProjectDiagram>

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        var diffObj = (DiffResultUpdatedItems<T>)obj;
        T source = this.Source;
        T target = this.Target;
        return target != null && source != null && source.Equals(diffObj.Source) && target.Equals(diffObj.Target);
    }

    /// <inheritdoc />
    public override int GetHashCode() => this.Source?.GetHashCode() ^ this.Target?.GetHashCode() ?? 0;

    /// <summary>
    /// Compares two <see cref="DiffResultUpdatedItems{T}" /> objects.
    /// </summary>
    /// <param name="left">A <see cref="DiffResultUpdatedItems{T}" /> object instance.</param>
    /// <param name="right">A <see cref="DiffResultUpdatedItems{T}" /> object instance.</param>
    /// <returns><c>True</c> if two objects are equal, <c>false</c> otherwise.</returns>
    public static bool operator ==(DiffResultUpdatedItems<T>? left, DiffResultUpdatedItems<T>? right) => Equals(left, right);

    /// <summary>
    /// Compares two <see cref="DiffResultUpdatedItems{T}" /> objects for inequality.
    /// </summary>
    /// <param name="left">A <see cref="DiffResultUpdatedItems{T}" /> instance.</param>
    /// <param name="right">A <see cref="DiffResultUpdatedItems{T}" /> instance.</param>
    /// <returns><c>True</c> if two values are not equal, <c>false</c> otherwise.</returns>
    public static bool operator !=(DiffResultUpdatedItems<T>? left, DiffResultUpdatedItems<T>? right) => !Equals(left, right);

    #endregion

    /// <inheritdoc />
    public override string ToString() => this.Source + "; " + this.Target;
}
