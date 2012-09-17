using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.List.Values;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Query.Values
{
    public sealed class GroupByValuesQueryResultAction : IMatchingObjectAction
    {
        private readonly string[] _groupByFieldList;

        /// <summary>
        ///   When executing a group by result, results are temporary stored in a hash map and at the end transfered to a Values objects In this case, the key of the map is the group by composed key, the value is a ValuesQueryResultAction
        /// </summary>
        private readonly IDictionary<IOdbComparable, ValuesQueryResultAction> _groupByResult;

        /// <summary>
        ///   An object to build instances
        /// </summary>
        private readonly IInstanceBuilder _instanceBuilder;

        private readonly IValuesQuery _query;

        private readonly bool _queryHasOrderBy;

        private readonly int _returnArraySize;
        private IValues _result;

        public GroupByValuesQueryResultAction(IValuesQuery query, IInstanceBuilder instanceBuilder)
        {
            _query = query;
            _queryHasOrderBy = query.HasOrderBy();
            _instanceBuilder = instanceBuilder;
            _returnArraySize = query.GetObjectActions().Count;
            _groupByFieldList = query.GetGroupByFieldList();
            _groupByResult = new OdbHashMap<IOdbComparable, ValuesQueryResultAction>();
        }

        #region IMatchingObjectAction Members

        public void ObjectMatch(OID oid, IOdbComparable orderByKey)
        {
        }

        // This method os not used in Values Query API
        public void ObjectMatch(OID oid, object @object, IOdbComparable orderByKey)
        {
            var values = (AttributeValuesMap) @object;
            var groupByKey = IndexTool.BuildIndexKey("GroupBy", values, _groupByFieldList);
            var result = _groupByResult[groupByKey];

            if (result == null)
            {
                result = new ValuesQueryResultAction(_query, null, _instanceBuilder);
                result.Start();
                _groupByResult.Add(groupByKey, result);
            }

            result.ObjectMatch(oid, @object, orderByKey);
        }

        public void Start()
        {
        }

        // Nothing to do
        public void End()
        {
            if (_query != null && _query.HasOrderBy())
                _result = new InMemoryBTreeCollectionForValues(_query.GetOrderByType());
            else
                _result = new SimpleListForValues(_returnArraySize);

            foreach (var key in _groupByResult.Keys)
            {
                var vqra = _groupByResult[key];
                vqra.End();
                Merge(key, vqra.GetValues());
            }
        }

        public IObjects<T> GetObjects<T>()
        {
            return (IObjects<T>) _result;
        }

        #endregion

        private void Merge(IOdbComparable key, IValues values)
        {
            while (values.HasNext())
            {
                if (_queryHasOrderBy)
                    _result.AddWithKey(key, values.NextValues());
                else
                    _result.Add(values.NextValues());
            }
        }
    }
}