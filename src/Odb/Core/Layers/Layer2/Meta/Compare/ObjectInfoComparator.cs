using System.Collections.Generic;
using System.Text;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Manage Object info differences.
    /// </summary>
    /// <remarks>
    ///   Manage Object info differences. compares two object info and tells which objects in the object hierarchy has changed. This is used by the update to process to optimize it and actually update what has changed
    /// </remarks>
    internal sealed class ObjectInfoComparator : IObjectInfoComparator
    {
        private const int Size = 5;
        private readonly IDictionary<NonNativeObjectInfo, int> _alreadyCheckingObjects;
        private readonly IList<ArrayModifyElement> _arrayChanges;

        private readonly IList<SetAttributeToNullAction> _attributeToSetToNull;

        private readonly IList<NonNativeObjectInfo> _changedObjectMetaRepresentations;

        private readonly IList<ChangedObjectInfo> _changes;

        private readonly IList<NewNonNativeObjectAction> _newObjectMetaRepresentations;
        private readonly IList<object> _newObjects;
        private readonly IList<IChangedAttribute> _changedAttributeActions;
        private int _maxObjectRecursionLevel;

        private int _nbChanges;

        public ObjectInfoComparator()
        {
            _changedObjectMetaRepresentations = new List<NonNativeObjectInfo>(Size);
            _attributeToSetToNull = new List<SetAttributeToNullAction>(Size);
            _alreadyCheckingObjects = new OdbHashMap<NonNativeObjectInfo, int>(Size);
            _newObjects = new List<object>(Size);
            _newObjectMetaRepresentations = new List<NewNonNativeObjectAction>(Size);
            _changes = new List<ChangedObjectInfo>(Size);
            _changedAttributeActions = new List<IChangedAttribute>(Size);
            _arrayChanges = new List<ArrayModifyElement>();
            _maxObjectRecursionLevel = 0;
        }

        #region IObjectInfoComparator Members

        public bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2)
        {
            return HasChanged(aoi1, aoi2, -1);
        }

        public IList<object> GetNewObjects()
        {
            return _newObjects;
        }

        public int GetMaxObjectRecursionLevel()
        {
            return _maxObjectRecursionLevel;
        }

        public IList<IChangedAttribute> GetChangedAttributeActions()
        {
            return _changedAttributeActions;
        }

        public IList<SetAttributeToNullAction> GetAttributeToSetToNull()
        {
            return _attributeToSetToNull;
        }

        public void Clear()
        {
            _changedObjectMetaRepresentations.Clear();
            _attributeToSetToNull.Clear();
            _alreadyCheckingObjects.Clear();
            _newObjects.Clear();
            _newObjectMetaRepresentations.Clear();
            _changes.Clear();
            _changedAttributeActions.Clear();
            _arrayChanges.Clear();
            _maxObjectRecursionLevel = 0;
            _nbChanges = 0;
        }

        public int GetNbChanges()
        {
            return _nbChanges;
        }

        public IList<ArrayModifyElement> GetArrayChanges()
        {
            return _arrayChanges;
        }

        #endregion

        private bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2, int objectRecursionLevel)
        {
            // If one is null and the other not
            if (aoi1.IsNull() != aoi2.IsNull())
                return true;
            if (aoi1.IsNonNativeObject() && aoi2.IsNonNativeObject())
                return HasChanged((NonNativeObjectInfo) aoi1, (NonNativeObjectInfo) aoi2, objectRecursionLevel + 1);
            if (aoi1.IsNative() && aoi2.IsNative())
                return HasChanged((NativeObjectInfo) aoi1, (NativeObjectInfo) aoi2);
            return false;
        }

        private static bool HasChanged(NativeObjectInfo aoi1, NativeObjectInfo aoi2)
        {
            if (aoi1.GetObject() == null && aoi2.GetObject() == null)
                return false;
            if (aoi1.GetObject() == null || aoi2.GetObject() == null)
                return true;

            return !aoi1.GetObject().Equals(aoi2.GetObject());
        }

        private bool HasChanged(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int objectRecursionLevel)
        {
            var hasChanged = false;
            // If the object is already being checked, return false, this second
            // check will not affect the check
            int n;
            _alreadyCheckingObjects.TryGetValue(nnoi2, out n);
            if (n != 0)
                return false;
            // Put the object in the temporary cache
            _alreadyCheckingObjects[nnoi1] = 1;
            _alreadyCheckingObjects[nnoi2] = 1;
            // Warning ID Start with 1 and not 0
            for (var id = 1; id <= nnoi1.GetMaxNbattributes(); id++)
            {
                var value1 = nnoi1.GetAttributeValueFromId(id);
                // Gets the value by the attribute id to be sure
                // Problem because a new object info may not have the right ids ?
                // Check if
                // the new oiD is ok.
                var value2 = nnoi2.GetAttributeValueFromId(id);
                if (value2 == null)
                {
                    // this means the object to have attribute id
                    StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                    hasChanged = true;
                    continue;
                }
                if (value1 == null)
                {
                    //throw new ODBRuntimeException("ObjectInfoComparator.hasChanged:attribute with id "+id+" does not exist on "+nnoi2);
                    // This happens when this object was created with an version of ClassInfo (which has been refactored).
                    // In this case,we simply tell that in place update is not supported so that the object will be rewritten with 
                    // new metamodel
                    continue;
                }
                // If both are null, no effect
                if (value1.IsNull() && value2.IsNull())
                    continue;
                if (value2.IsNull())
                {
                    hasChanged = true;
                    StoreActionSetAttributetoNull();
                    continue;
                }
                if (value1.IsNull() && value2.IsNonNativeObject())
                {
                    hasChanged = true;
                    StoreNewObjectReference();
                    continue;
                }
                if (!ClassAreCompatible(value1, value2))
                {
                    var nativeObjectInfo = value2 as NativeObjectInfo;
                    if (nativeObjectInfo != null)
                    {
                        StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                        StoreChangedAttributeAction(new ChangedNativeAttributeAction());
                    }
                    var objectReference = value2 as ObjectReference;
                    if (objectReference != null)
                    {
                        var nnoi = (NonNativeObjectInfo) value1;
                        var oref = objectReference;
                        if (!nnoi.GetOid().Equals(oref.GetOid()))
                        {
                            StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                            StoreChangedAttributeAction(new ChangedObjectReferenceAttributeAction());
                        }
                        else
                            continue;
                    }
                    hasChanged = true;
                    continue;
                }
                if (value1.IsAtomicNativeObject())
                {
                    if (!value1.Equals(value2))
                    {
                        StoreChangedAttributeAction(new ChangedNativeAttributeAction());
                        hasChanged = true;
                    }
                    continue;
                }
                if (value1.IsArrayObject())
                {
                    var aoi1 = (ArrayObjectInfo) value1;
                    var aoi2 = (ArrayObjectInfo) value2;
                    var arrayHasChanged = ManageArrayChanges(nnoi1, nnoi2, id, aoi1, aoi2, objectRecursionLevel);
                    hasChanged = hasChanged || arrayHasChanged;
                    continue;
                }
                if (value1.IsEnumObject())
                {
                    var enoi1 = (EnumNativeObjectInfo) value1;
                    var enoi2 = (EnumNativeObjectInfo) value2;
                    var enumHasChanged = !enoi1.GetEnumClassInfo().ClassInfoId.Equals(enoi2.GetEnumClassInfo().ClassInfoId) ||
                                          !enoi1.GetEnumName().Equals(enoi2.GetEnumName());
                    hasChanged = hasChanged || enumHasChanged;
                    continue;
                }
                if (value1.IsNonNativeObject())
                {
                    var oi1 = (NonNativeObjectInfo) value1;
                    var oi2 = (NonNativeObjectInfo) value2;
                    // If oids are equal, they are the same objects
                    if (oi1.GetOid() != null && oi1.GetOid().Equals(oi2.GetOid()))
                        hasChanged = HasChanged(value1, value2, objectRecursionLevel + 1) || hasChanged;
                    else
                    {
                        // This means that an object reference has changed.
                        hasChanged = true;
                        StoreNewObjectReference();
                        objectRecursionLevel++;
                    }
                }
            }
            var i1 = _alreadyCheckingObjects[nnoi1];
            var i2 = _alreadyCheckingObjects[nnoi2];
            i1 = i1 - 1;
            i2 = i2 - 1;
            if (i1 == 0)
                _alreadyCheckingObjects.Remove(nnoi1);
            else
                _alreadyCheckingObjects.Add(nnoi1, i1);
            if (i2 == 0)
                _alreadyCheckingObjects.Remove(nnoi2);
            else
                _alreadyCheckingObjects.Add(nnoi2, i2);
            return hasChanged;
        }

        private void StoreNewObjectReference()
        {
            _newObjectMetaRepresentations.Add(new NewNonNativeObjectAction());
            _nbChanges++;
        }

        private void StoreActionSetAttributetoNull()
        {
            _nbChanges++;
            var action = new SetAttributeToNullAction();
            _attributeToSetToNull.Add(action);
        }

        private void StoreArrayChange()
        {
            _nbChanges++;
            var ame = new ArrayModifyElement();
            _arrayChanges.Add(ame);
        }

        private static bool ClassAreCompatible(AbstractObjectInfo value1, AbstractObjectInfo value2)
        {
            var clazz1 = value1.GetType();
            var clazz2 = value2.GetType();

            return clazz1 == clazz2;
        }

        private void StoreChangedObject(NonNativeObjectInfo aoi1, NonNativeObjectInfo aoi2, int fieldId,
                                        AbstractObjectInfo oldValue, AbstractObjectInfo newValue, string message,
                                        int objectRecursionLevel)
        {
            if (aoi1 != null && aoi2 != null)
            {
                if (aoi1.GetOid() != null && aoi1.GetOid().Equals(aoi2.GetOid()))
                {
                    _changedObjectMetaRepresentations.Add(aoi2);
                    _changes.Add(new ChangedObjectInfo(aoi1.GetClassInfo(), aoi2.GetClassInfo(), fieldId, oldValue,
                                                       newValue, message, objectRecursionLevel));
                    // also the max recursion level
                    if (objectRecursionLevel > _maxObjectRecursionLevel)
                        _maxObjectRecursionLevel = objectRecursionLevel;
                    _nbChanges++;
                }
                else
                {
                    _newObjects.Add(aoi2.GetObject());
                    StoreNewObjectReference();
                }
            }
            else
            {
                //newObjectMetaRepresentations.add(aoi2);
                DLogger.Info("Non native object with null object");
            }
        }

        private void StoreChangedObject(NonNativeObjectInfo aoi1, NonNativeObjectInfo aoi2, int fieldId,
                                        int objectRecursionLevel)
        {
            _nbChanges++;
            if (aoi1 != null && aoi2 != null)
            {
                _changes.Add(new ChangedObjectInfo(aoi1.GetClassInfo(), aoi2.GetClassInfo(), fieldId,
                                                   aoi1.GetAttributeValueFromId(fieldId),
                                                   aoi2.GetAttributeValueFromId(fieldId), objectRecursionLevel));
                // also the max recursion level
                if (objectRecursionLevel > _maxObjectRecursionLevel)
                    _maxObjectRecursionLevel = objectRecursionLevel;
            }
            else
                DLogger.Info("Non native object with null object");
        }

        /// <summary>
        ///   Checks if something in the Arary has changed, if yes, stores the change
        /// </summary>
        /// <param name="nnoi1"> The first Object meta representation (nnoi = NonNativeObjectInfo) </param>
        /// <param name="nnoi2"> The second object meta representation </param>
        /// <param name="fieldId"> The field index that this collection represents </param>
        /// <param name="aoi1"> The Meta representation of the array 1 (aoi = ArraybjectInfo) </param>
        /// <param name="aoi2"> The Meta representation of the array 2 </param>
        /// <param name="objectRecursionLevel"> </param>
        /// <returns> true if the 2 array representations are different </returns>
        private bool ManageArrayChanges(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int fieldId,
                                        ArrayObjectInfo aoi1, ArrayObjectInfo aoi2, int objectRecursionLevel)
        {
            var array1 = aoi1.GetArray();
            var array2 = aoi2.GetArray();
            if (array1.Length != array2.Length)
            {
                var buffer = new StringBuilder();
                buffer.Append("Array size has changed oldsize=").Append(array1.Length).Append("/newsize=").Append(
                    array2.Length);
                StoreChangedObject(nnoi1, nnoi2, fieldId, aoi1, aoi2, buffer.ToString(), objectRecursionLevel);
                return true;
            }
            
            for (var i = 0; i < array1.Length; i++)
            {
                var value1 = (AbstractObjectInfo) array1[i];
                var value2 = (AbstractObjectInfo) array2[i];
                var localHasChanged = HasChanged(value1, value2, objectRecursionLevel);
                if (!localHasChanged) 
                    continue;

                StoreArrayChange();
                return true;
            }
            return false;
        }

        private void StoreChangedAttributeAction(IChangedAttribute caa)
        {
            _nbChanges++;
            _changedAttributeActions.Add(caa);
        }

        public override string ToString()
        {
            return string.Format("{0} changes", _nbChanges);
        }
    }
}
