using System;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NonPersistentAttribute : Attribute
    {
    }
}