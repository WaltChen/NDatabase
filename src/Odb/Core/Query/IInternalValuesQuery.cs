using System.Collections.Generic;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query
{
    internal interface IInternalValuesQuery : IValuesQuery
    {
        IEnumerable<IQueryFieldAction> GetObjectActions();
        string[] GetGroupByFieldList();

        bool HasGroupBy();

        /// <summary>
        ///   To indicate if a query will return one row (for example, sum, average, max and min, or will return more than one row
        /// </summary>
        bool IsMultiRow();

        int ObjectActionsCount { get; }

        /// <summary>
        ///   used with isForSingleOid == true, to indicate we are working on a single object with a specific oid
        /// </summary>
        /// <returns> </returns>
        OID GetOidOfObjectToQuery();

        bool ReturnInstance();

        /// <summary>
        ///   Returns true if the query has an order by clause
        /// </summary>
        /// <returns> true if has an order by flag </returns>
        bool HasOrderBy();
    }
}