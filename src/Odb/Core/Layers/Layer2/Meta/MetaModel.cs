using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NDatabase.Exceptions;
using NDatabase.Tool.Wrappers;
using NDatabase.TypeResolution;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   The database meta-model
    /// </summary>
    internal sealed class MetaModel : IMetaModel
    {
        /// <summary>
        ///   A simple list to hold all class infos.
        /// </summary>
        /// <remarks>
        ///   A simple list to hold all class infos. It is redundant with the maps, but in some cases, we need sequential access to classes :-(
        /// </remarks>
        private readonly IList<ClassInfo> _allClassInfos;

        /// <summary>
        ///   A list of changed classes - that must be persisted back when commit is done
        /// </summary>
        private readonly OdbHashMap<ClassInfo, ClassInfo> _changedClasses;

        private readonly IDictionary<OID, ClassInfo> _rapidAccessForClassesByOid;
        private readonly IList<Type> _existingClasses;

        /// <summary>
        ///   to identify if meta model has changed
        /// </summary>
        private bool _hasChanged;

        /// <summary>
        ///   A hash map to speed up the access of class info by full class name
        /// </summary>
        private IDictionary<string, ClassInfo> _rapidAccessForClassesByName;

        public MetaModel()
        {
            _rapidAccessForClassesByName = new OdbHashMap<string, ClassInfo>(10);
            _rapidAccessForClassesByOid = new OdbHashMap<OID, ClassInfo>(10);
            _existingClasses = new List<Type>(10);
            _allClassInfos = new List<ClassInfo>();
            _changedClasses = new OdbHashMap<ClassInfo, ClassInfo>();
        }

        public void AddClass(ClassInfo classInfo)
        {
            var fullClassName = classInfo.FullClassName;

            _rapidAccessForClassesByName.Add(fullClassName, classInfo);
            _existingClasses.Add(classInfo.UnderlyingType);
            _rapidAccessForClassesByOid.Add(classInfo.ClassInfoId, classInfo);
            _allClassInfos.Add(classInfo);
        }

        public void AddClasses(ClassInfoList ciList)
        {
            foreach (var classInfo in ciList.GetClassInfos())
                AddClass(classInfo);
        }

        public bool ExistClass(Type type)
        {
            return _existingClasses.Contains(type);
        }

        public override string ToString()
        {
            return string.Format("{0}", _rapidAccessForClassesByName.Values);
        }

        public IEnumerable<ClassInfo> GetAllClasses()
        {
            return _allClassInfos;
        }

        public int GetNumberOfClasses()
        {
            return _allClassInfos.Count;
        }

        /// <summary>
        ///   Gets the class info from the OID.
        /// </summary>
        /// <remarks>
        ///   Gets the class info from the OID.
        /// </remarks>
        /// <param name="id"> </param>
        /// <returns> the class info with the OID </returns>
        public ClassInfo GetClassInfoFromId(OID id)
        {
            return _rapidAccessForClassesByOid[id];
        }

        public ClassInfo GetClassInfo(Type type, bool throwExceptionIfDoesNotExist)
        {
            var fullClassName = OdbClassUtil.GetFullName(type);
            return GetClassInfo(fullClassName, throwExceptionIfDoesNotExist);
        }

        public ClassInfo GetClassInfo(string fullClassName, bool throwExceptionIfDoesNotExist)
        {
            ClassInfo classInfo;
            
            _rapidAccessForClassesByName.TryGetValue(fullClassName, out classInfo);
            if (classInfo != null)
                return classInfo;

            if (throwExceptionIfDoesNotExist)
                throw new OdbRuntimeException(
                    NDatabaseError.MetaModelClassNameDoesNotExist.AddParameter(fullClassName));

            return null;
        }

        /// <returns> The Last class info </returns>
        public ClassInfo GetLastClassInfo()
        {
            return _allClassInfos[_allClassInfos.Count - 1];
        }

        /// <param name="index"> The index of the class info to get </param>
        /// <returns> The class info at the specified index </returns>
        public ClassInfo GetClassInfo(int index)
        {
            return _allClassInfos[index];
        }

        public void Clear()
        {
            _rapidAccessForClassesByName.Clear();
            _rapidAccessForClassesByName = null;
            _allClassInfos.Clear();
        }

        public bool HasChanged()
        {
            return _hasChanged;
        }

        public IEnumerable<ClassInfo> GetChangedClassInfo()
        {
            IOdbList<ClassInfo> list = new OdbList<ClassInfo>();
            list.AddAll(_changedClasses.Keys);

            return new ReadOnlyCollection<ClassInfo>(list);
        }

        public void ResetChangedClasses()
        {
            _changedClasses.Clear();
            _hasChanged = false;
        }

        /// <summary>
        ///   Saves the fact that something has changed in the class (number of objects or last object oid)
        /// </summary>
        public void AddChangedClass(ClassInfo classInfo)
        {
            _changedClasses[classInfo] = classInfo;
            _hasChanged = true;
        }

        /// <summary>
        ///   Gets all the persistent classes that are subclasses or equal to the parameter class
        /// </summary>
        /// <returns> The list of class info of persistent classes that are subclasses or equal to the class </returns>
        public IList<ClassInfo> GetPersistentSubclassesOf(Type type)
        {
            var result = new List<ClassInfo>();

            var fullClassName = OdbClassUtil.GetFullName(type);
            var theClass = type;

            CheckList(fullClassName, result, theClass, _rapidAccessForClassesByName);

            return result;
        }

        private void CheckList(string fullClassName, ICollection<ClassInfo> result, Type theClass, IDictionary<string, ClassInfo> listToCheck)
        {
            foreach (var userClass in listToCheck.Keys)
            {
                if (userClass.Equals(fullClassName))
                {
                    result.Add(GetClassInfo(userClass, true));
                }
                else
                {
                    var oneClass = TypeResolutionUtils.ResolveType(userClass);
                    if (theClass.IsAssignableFrom(oneClass))
                        result.Add(GetClassInfo(userClass, true));
                }
            }
        }
    }
}
