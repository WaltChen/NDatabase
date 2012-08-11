using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   A callback interface - not used
    /// </summary>
    /// <author>osmadja</author>
    public interface IObjectWriterCallback
    {
        void MetaObjectHasBeenInserted(long oid, NonNativeObjectInfo nnoi);

        void MetaObjectHasBeenUpdated(long oid, NonNativeObjectInfo nnoi);
    }
}
