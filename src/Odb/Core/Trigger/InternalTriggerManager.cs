using System;
using System.Collections.Generic;
using NDatabase.Exceptions;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Main;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Trigger
{
    internal sealed class InternalTriggerManager : IInternalTriggerManager
    {
        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfDeleteTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfInsertTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfSelectTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfUpdateTriggers;

        private readonly IStorageEngine _storageEngine;

        public InternalTriggerManager(IStorageEngine engine)
        {
            _storageEngine = engine;
            _listOfUpdateTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfDeleteTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfSelectTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfInsertTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
        }

        #region IInternalTriggerManager Members

        public void AddUpdateTriggerFor(Type type, UpdateTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfUpdateTriggers);
        }

        public void AddInsertTriggerFor(Type type, InsertTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfInsertTriggers);
        }

        public void AddDeleteTriggerFor(Type type, DeleteTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfDeleteTriggers);
        }

        public void AddSelectTriggerFor(Type type, SelectTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfSelectTriggers);
        }

        public void ManageInsertTriggerBefore(Type type, object @object)
        {
            if (!HasInsertTriggersFor(type))
                return;

            foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    if (@object != null)
                        trigger.BeforeInsert(@object);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.BeforeInsertTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                            .AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageInsertTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasInsertTriggersFor(type))
                return;

            foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.AfterInsert(@object, oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterInsertTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageUpdateTriggerBefore(Type type, NonNativeObjectInfo oldNnoi, object newObject, OID oid)
        {
            if (!HasUpdateTriggersFor(type))
                return;

            foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    var storageEngine = ((OdbAdapter) trigger.Odb).GetStorageEngine();
                    trigger.BeforeUpdate(new ObjectRepresentation(oldNnoi, storageEngine.ClassInfoProvider), newObject, oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.BeforeUpdateTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                            .AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageUpdateTriggerAfter(Type type, NonNativeObjectInfo oldNnoi, object newObject, OID oid)
        {
            if (!HasUpdateTriggersFor(type))
                return;

            foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    var storageEngine = ((OdbAdapter)trigger.Odb).GetStorageEngine();
                    trigger.AfterUpdate(new ObjectRepresentation(oldNnoi, storageEngine.ClassInfoProvider), newObject, oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterUpdateTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageDeleteTriggerBefore(Type type, object @object, OID oid)
        {
            if (!HasDeleteTriggersFor(type))
                return;

            foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.BeforeDelete(@object, oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.BeforeDeleteTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                            .AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageDeleteTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasDeleteTriggersFor(type))
                return;

            foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.AfterDelete(@object, oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterDeleteTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageSelectTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasSelectTriggersFor(type))
                return;

            foreach (SelectTrigger trigger in GetListOfSelectTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                if (@object != null)
                    trigger.AfterSelect(@object, oid);
            }
        }

        #endregion

        private bool HasDeleteTriggersFor(Type type)
        {
            return _listOfDeleteTriggers.ContainsKey(type) || _listOfDeleteTriggers.ContainsKey(typeof (object));
        }

        private bool HasInsertTriggersFor(Type type)
        {
            return _listOfInsertTriggers.ContainsKey(type) || _listOfInsertTriggers.ContainsKey(typeof (object));
        }

        private bool HasSelectTriggersFor(Type type)
        {
            return _listOfSelectTriggers.ContainsKey(type) || _listOfSelectTriggers.ContainsKey(typeof (object));
        }

        private bool HasUpdateTriggersFor(Type type)
        {
            return _listOfUpdateTriggers.ContainsKey(type) || _listOfUpdateTriggers.ContainsKey(typeof (object));
        }

        private static void AddTriggerFor<TTrigger>(Type type, TTrigger trigger,
                                                    IDictionary<Type, IOdbList<Trigger>> listOfTriggers)
            where TTrigger : Trigger
        {
            var triggers = listOfTriggers[type];

            if (triggers == null)
            {
                triggers = new OdbList<Trigger>();
                listOfTriggers.Add(type, triggers);
            }

            triggers.Add(trigger);
        }

        private IEnumerable<Trigger> GetListOfDeleteTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfDeleteTriggers);
        }

        private IEnumerable<Trigger> GetListOfInsertTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfInsertTriggers);
        }

        private IEnumerable<Trigger> GetListOfSelectTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfSelectTriggers);
        }

        private IEnumerable<Trigger> GetListOfUpdateTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfUpdateTriggers);
        }

        private static IEnumerable<Trigger> GetListOfTriggersFor(Type type,
                                                                 IDictionary<Type, IOdbList<Trigger>> listOfTriggers)
        {
            var listOfTriggersBuClassName = listOfTriggers[type];
            var listOfTriggersByAllClassTrigger = listOfTriggers[typeof (object)];

            if (listOfTriggersByAllClassTrigger != null)
            {
                var size = listOfTriggersByAllClassTrigger.Count;
                if (listOfTriggersBuClassName != null)
                    size = size + listOfTriggersBuClassName.Count;

                var listOfTriggersToReturn = new OdbList<Trigger>(size);

                if (listOfTriggersBuClassName != null)
                {
                    listOfTriggersToReturn.AddRange(listOfTriggersBuClassName);
                }

                listOfTriggersToReturn.AddRange(listOfTriggersByAllClassTrigger);
                return listOfTriggersToReturn;
            }

            return listOfTriggersBuClassName;
        }
    }
}