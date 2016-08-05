﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository abstraction.
    /// <typeparam name="TEntity">The entity repository wraps.</typeparam>
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get entity instance by id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Entity instance.</returns>
        TEntity Get(object id);

        /// <summary>
        /// Get all entities of specified type.
        /// </summary>
        /// <returns>Enumerable of entities.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Finds for range of entities based on predicate.
        /// </summary>
        /// <param name="predicate">Filter predicate.</param>
        /// <returns>Enumerable of enitites.</returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Add entity.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Add range of entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Remove entity instance.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Remove range of entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
