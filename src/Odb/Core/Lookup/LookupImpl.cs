using System.Collections.Generic;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Lookup
{
    /// <summary>
    ///   A simple class to enable direct object lookup by object id
    /// </summary>
    /// <author>olivier</author>
    public class LookupImpl : ILookup
    {
        private readonly IDictionary<string, object> _objects;

        public LookupImpl()
        {
            _objects = new OdbHashMap<string, object>();
        }

        #region ILookup Members

        public virtual object Get(string objectId)
        {
            return _objects[objectId];
        }

        public virtual void Set(string objectId, object @object)
        {
            _objects.Add(objectId, @object);
        }

        public virtual int Size()
        {
            return _objects.Count;
        }

        #endregion
    }
}
