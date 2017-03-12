﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.EF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;
    using Domain;

    /// <summary>
    /// Entity Framework repository implementation that supports <see cref="IQueryable" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TContext">Database context type.</typeparam>
    public class EFQueryableRepository<TEntity, TContext> : EFRepository<TEntity, TContext>, IQueryableRepository<TEntity>
        where TEntity : class where TContext : DbContext
    {
        private readonly DbSet<TEntity> set;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="context">Database context.</param>
        public EFQueryableRepository([NotNull] TContext context) : base(context)
        {
            set = Context.Set<TEntity>();
        }

        /// <inheritdoc />
        public Type ElementType => typeof(TEntity);

        /// <inheritdoc />
        public Expression Expression => ((IQueryable<TEntity>)set).Expression;

        /// <inheritdoc />
        public IQueryProvider Provider => ((IQueryable<TEntity>)set).Provider;

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator() => ((IQueryable<TEntity>)set).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IQueryable<TEntity>)set).GetEnumerator();
    }
}
