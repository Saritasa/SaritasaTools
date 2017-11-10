﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Common.Repositories.QueryProviders
{
    using System;
    using System.Linq;
    using System.Text;
    using Internal;
    using Internal.Clauses;
    using Internal.Enums;

    /// <summary>
    /// The SELECT statement MySQL builder.
    /// </summary>
    /// <seealso cref="Saritasa.Tools.Messages.Internal.SelectStringBuilder" />
    internal class MySqlSelectStringBuilder : SelectStringBuilder
    {
        /// <inheritdoc />
        public override string Build()
        {
            var sb = new StringBuilder("SELECT ");

            // Output Distinct.
            if (IsDistinct)
            {
                sb.Append("DISTINCT ");
            }

            // Output column names.
            sb.Append(SelectedColumns.Any() ? string.Join(", ", SelectedColumns.Select(WrapVariable)) : "*");

            // Output table names.
            if (SelectedTables.Any())
            {
                sb.Append($" FROM {string.Join(", ", SelectedTables.Select(WrapVariable))}");
            }

            // Output joins.
            if (JoinStatement.Any())
            {
                foreach (var clause in JoinStatement)
                {
                    sb.AppendLine();
                    switch (clause.JoinType)
                    {
                        case JoinType.InnerJoin:
                            sb.Append("INNER JOIN ");
                            break;
                        case JoinType.LeftJoin:
                            sb.Append("LEFT JOIN ");
                            break;
                        case JoinType.RightJoin:
                            sb.Append("RIGHT JOIN ");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(clause.JoinType), $"MySql doesn't support {clause.JoinType} join type.");
                    }
                    sb.Append($"`{clause.ToTable}` ON ");
                    sb.Append(CreateComparisonClause(
                        $"{clause.ToTable}.{clause.ToColumn}",
                        clause.ComparisonOperator,
                        new SqlLiteral($"{clause.FromTable}.{clause.FromColumn}")));
                }
            }

            // Output where statement.
            if (WhereStatement.Any())
            {
                sb.AppendLine();
                sb.Append($"WHERE {string.Join(" AND ", WhereStatement.Select(BuildWhereClauseString))}");
            }

            // Output GroupBy statement.
            if (GroupByColumns.Count > 0)
            {
                sb.AppendLine();
                sb.Append($"GROUP BY {string.Join(", ", GroupByColumns.Select(WrapVariable))}");
            }

            // Output OrderBy statement.
            if (OrderByStatement.Any())
            {
                sb.AppendLine();
                sb.Append($"ORDER BY {string.Join(", ", OrderByStatement.Select(BuildOrderByClauseString))}");
            }

            if (TakeRows.HasValue)
            {
                sb.AppendLine();

                if (SkipRows.HasValue)
                {
                    sb.Append($"LIMIT {SkipRows}, {TakeRows}");
                }
                else
                {
                    sb.Append($"LIMIT {TakeRows}");
                }
            }

            // Return the built query.
            return sb.ToString();
        }

        private static string BuildOrderByClauseString(OrderByClause clause)
        {
            return clause.SortOrder == SortingOperator.Descending
                ? $"{WrapVariable(clause.ColumnName)} DESC"
                : $"{WrapVariable(clause.ColumnName)}";
        }

        private static string BuildWhereClauseString(WhereClause clause)
        {
            var sb = new StringBuilder();

            sb.Append(CreateComparisonClause(clause.ColumnName, clause.Operator, clause.Value));

            return $"({sb})";
        }

        private static string CreateComparisonClause(string columnName, ComparisonOperator comparisonOperatorOperator, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                switch (comparisonOperatorOperator)
                {
                    case ComparisonOperator.Equals:
                        return $"{WrapVariable(columnName)} IS NULL";
                    case ComparisonOperator.NotEquals:
                        return $"NOT {WrapVariable(columnName)} IS NULL";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(comparisonOperatorOperator),
                            string.Format(Properties.Strings.CannotUseComparisonOperatorNull, comparisonOperatorOperator));
                }
            }

            switch (comparisonOperatorOperator)
            {
                case ComparisonOperator.Equals:
                    return $"{WrapVariable(columnName)} = {FormatSqlValue(value)}";
                case ComparisonOperator.NotEquals:
                    return $"{WrapVariable(columnName)} <> {FormatSqlValue(value)}";
                case ComparisonOperator.GreaterThan:
                    return $"{WrapVariable(columnName)} > {FormatSqlValue(value)}";
                case ComparisonOperator.GreaterOrEquals:
                    return $"{WrapVariable(columnName)} >= {FormatSqlValue(value)}";
                case ComparisonOperator.LessThan:
                    return $"{WrapVariable(columnName)} < {FormatSqlValue(value)}";
                case ComparisonOperator.LessOrEquals:
                    return $"{WrapVariable(columnName)} <= {FormatSqlValue(value)}";
                case ComparisonOperator.Like:
                    return $"{WrapVariable(columnName)} LIKE {FormatSqlValue(value)}";
                case ComparisonOperator.NotLike:
                    return $"NOT {WrapVariable(columnName)} LIKE {FormatSqlValue(value)}";
                case ComparisonOperator.In:
                    return $"{WrapVariable(columnName)} IN ({FormatSqlValue(value)})";
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparisonOperatorOperator),
                        string.Format(Properties.Strings.CannotUseComparisonOperator, comparisonOperatorOperator));
            }
        }

        private static string FormatSqlValue(object someValue)
        {
            if (someValue == null || someValue is DBNull)
            {
                return "NULL";
            }
            if (someValue is Guid)
            {
                return $"\'{(Guid)someValue}\'";
            }
            if (someValue is string)
            {
                return $"\'{((string)someValue).Replace("'", "''")}\'";
            }
            if (someValue is DateTime)
            {
                return $"\'{(DateTime)someValue:yyyy-MM-dd hh:mm:ss}\'";
            }
            if (someValue is bool)
            {
                return (bool)someValue ? "1" : "0";
            }
            if (someValue is SqlLiteral)
            {
                return WrapVariable(((SqlLiteral)someValue).Value);
            }

            return someValue.ToString();
        }

        private static string WrapVariable(string arg)
        {
            return string.Join(".", arg.Split('.').Select(s => $"`{s}`"));
        }
    }
}
