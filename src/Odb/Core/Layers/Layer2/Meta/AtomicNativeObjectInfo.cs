using System;
using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep info about a native object like int,char, long, Does not include array or collection
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public class AtomicNativeObjectInfo : NativeObjectInfo, IComparable
    {
        public AtomicNativeObjectInfo(object @object, int odbTypeId) : base(@object, odbTypeId)
        {
        }

        #region IComparable Members

        public virtual int CompareTo(object o)
        {
            var anoi = (AtomicNativeObjectInfo) o;
            var c2 = (IComparable) anoi.GetObject();
            var c1 = (IComparable) TheObject;
            return c1.CompareTo(c2);
        }

        #endregion

        public override string ToString()
        {
            return TheObject != null
                       ? TheObject.ToString()
                       : "null";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is AtomicNativeObjectInfo))
                return false;

            var noi = (AtomicNativeObjectInfo) obj;

            return TheObject == noi.GetObject() || TheObject.Equals(noi.GetObject());
        }

        public override int GetHashCode()
        {
            return TheObject.GetHashCode();
        }

        public override bool IsAtomicNativeObject()
        {
            return true;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            return new AtomicNativeObjectInfo(TheObject, OdbTypeId);
        }
    }
}