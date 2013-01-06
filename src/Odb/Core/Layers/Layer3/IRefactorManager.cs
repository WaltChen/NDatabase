using System;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   <p>An interface for refactoring</p>
    /// </summary>
    public interface IRefactorManager
    {
        /// <summary>
        /// Rename stored class - refactoring
        /// </summary>
        /// <param name="className">Old class name</param>
        /// <param name="newClassName">New class name</param>
        void RenameClass(string className, string newClassName);

        /// <summary>
        /// Rename field of stored class - refactoring
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="attributeName">Old attribute name</param>
        /// <param name="newAttributeName">New attribute name</param>
        void RenameField(Type type, string attributeName, string newAttributeName);

        /// <summary>
        /// Extend stored class by new field - refactoring
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="fieldType">New field type</param>
        /// <param name="fieldName">New field name</param>
        void AddField(Type type, Type fieldType, string fieldName);

        /// <summary>
        /// Remove field from stored class - refactoring
        /// </summary>
        /// <param name="type">Type of field to remove</param>
        /// <param name="attributeName">Name of field to remove</param>
        void RemoveField(Type type, string attributeName);
    }
}
