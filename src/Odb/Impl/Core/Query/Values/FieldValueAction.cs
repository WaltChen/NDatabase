using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Odb.Impl.Core.Query.List.Objects;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    /// <summary>
    ///   An action to retrieve an object field
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class FieldValueAction : AbstractQueryFieldAction
    {
        /// <summary>
        ///   The value of the attribute
        /// </summary>
        private object _value;

        public FieldValueAction(string attributeName, string alias) : base(attributeName, alias, true)
        {
            _value = null;
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            _value = values[AttributeName];
            if (OdbType.IsCollection(_value.GetType()))
            {
                // For collection,we encapsulate it in an lazy load list that will create objects on demand
                var c = (ICollection<object>) _value;
                var l = new LazySimpleListOfAoi<object>(GetInstanceBuilder(), ReturnInstance());
                l.AddAll(c);
                _value = l;
            }
        }

        public override object GetValue()
        {
            return _value;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(AttributeName).Append("=").Append(_value);
            return buffer.ToString();
        }

        public override void End()
        {
        }

        public override void Start()
        {
        }

        public override IQueryFieldAction Copy()
        {
            return new FieldValueAction(AttributeName, Alias);
        }
    }
}