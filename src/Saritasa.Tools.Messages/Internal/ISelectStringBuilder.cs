﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Saritasa.Tools.Messages.Internal.Clauses;
using Saritasa.Tools.Messages.Internal.Enums;

namespace Saritasa.Tools.Messages.Internal
{
    /// <summary>
    /// The SELECT statement builder interface.
    /// </summary>
    internal interface ISelectStringBuilder
    {
        /// <summary>
        /// Gets the selected columns.
        /// </summary>
        /// <value>
        /// The selected columns.
        /// </value>
        IList<string> SelectedColumns { get; }

        /// <summary>
        /// Gets a value indicating whether the SELECT statement has DISTINCT clause.
        /// </summary>
        /// <value>
        /// <c>true</c> if the SELECT statement has DISTINCT clause; otherwise, <c>false</c>.
        /// </value>
        bool IsDistinct { get; set; }

        /// <summary>
        /// Gets or sets the selected tables.
        /// </summary>
        /// <value>
        /// The selected tables.
        /// </value>
        IList<string> SelectedTables { get; }

        /// <summary>
        /// Gets the JOIN statement.
        /// </summary>
        /// <value>
        /// The JOIN statement.
        /// </value>
        IList<JoinClause> JoinStatement { get; }

        /// <summary>
        /// Gets the WHERE statement.
        /// </summary>
        /// <value>
        /// The WHERE statement.
        /// </value>
        IList<WhereClause> WhereStatement { get; }

        /// <summary>
        /// Gets the GROUP BY columns.
        /// </summary>
        /// <value>
        /// The GROUP BY columns.
        /// </value>
        IList<string> GroupByColumns { get; }

        /// <summary>
        /// Gets the ORDER BY statement.
        /// </summary>
        /// <value>
        /// The ORDER BY statement.
        /// </value>
        IList<OrderByClause> OrderByStatement { get; }

        /// <summary>
        /// Gets or sets the skipped rows count.
        /// </summary>
        /// <value>
        /// The skipped rows count.
        /// </value>
        int? SkipRows { get; set; }

        /// <summary>
        /// Gets or sets the taken rows count.
        /// </summary>
        /// <value>
        /// The taken rows count.
        /// </value>
        int? TakeRows { get; set; }

        /// <summary>
        /// Selects all columns.
        /// </summary>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder SelectAll();

        /// <summary>
        /// Selects the specified columns.
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder Select(params string[] columnNames);

        /// <summary>
        /// Adds the specified columns to the SELECT statement.
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder AddSelect(params string[] columnNames);

        /// <summary>
        /// Distincts result records.
        /// </summary>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder Distinct();

        /// <summary>
        /// Sets the specified table names.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder From(params string[] tableNames);

        /// <summary>
        /// Joins the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="joinType">Type of the join clause.</param>
        /// <returns>Join clause.</returns>
        JoinClause Join(string tableName, JoinType joinType = JoinType.InnerJoin);

        /// <summary>
        /// Joins the specified table with inner join type.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Join clause.</returns>
        JoinClause InnerJoin(string tableName);

        /// <summary>
        /// Joins the specified table with left join type.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Join clause.</returns>
        JoinClause LeftJoin(string tableName);

        /// <summary>
        /// Joins the specified table with right join type.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Join clause.</returns>
        JoinClause RightJoin(string tableName);

        /// <summary>
        /// Joins the specified table with full outer join type.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Join clause.</returns>
        JoinClause OuterJoin(string tableName);

        /// <summary>
        /// Sets GROUP BY columns.
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder GroupBy(params string[] columnNames);

        /// <summary>
        /// Sets WHERE statement for the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Where clause.</returns>
        WhereClause Where(string columnName);

        /// <summary>
        /// Sets WHERE statement for the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="value">The value.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder Where(string columnName, ComparisonOperator @operator, object value);

        /// <summary>
        /// Orders result.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder OrderBy(string columnName);

        /// <summary>
        /// Orders result.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="operator">The operator.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder OrderBy(string columnName, SortingOperator @operator);

        /// <summary>
        /// Skips the specified rows count.
        /// </summary>
        /// <param name="rows">The rows count.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder Skip(int rows);

        /// <summary>
        /// Takes the specified rows count.
        /// </summary>
        /// <param name="rows">The rows count.</param>
        /// <returns>Select string builder.</returns>
        ISelectStringBuilder Take(int rows);

        /// <summary>
        /// Builds the SELECT statement.
        /// </summary>
        /// <returns>The SELECT statement string.</returns>
        string Build();
    }
}
