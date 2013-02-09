using System.Collections.Generic;
using System.Diagnostics;

namespace NDatabase.Tool
{
    /// <summary>
    ///   Simple logging class
    /// </summary>
    internal static class DLogger
    {
        private static readonly IList<ILogger> Loggers = new List<ILogger>();

        internal static void Register(ILogger logger)
        {
            Loggers.Add(logger);
        }

        internal static void Warning(object @object)
        {
            var type = new StackFrame(1).GetMethod().DeclaringType.Name;
            
            foreach (var logger in Loggers)
            {
                logger.Info(string.Concat(type, ": "));
                logger.Warning(@object == null
                                      ? "null"
                                      : @object.ToString());
            }
        }

        internal static void Debug(object @object)
        {
            var type = new StackFrame(1).GetMethod().DeclaringType.Name;

            foreach (var logger in Loggers)
            {
                logger.Info(string.Concat(type, ": "));
                logger.Debug(@object == null
                                      ? "null"
                                      : @object.ToString());
            }
        }

        internal static void Info(object @object)
        {
            var type = new StackFrame(1).GetMethod().DeclaringType.Name;

            foreach (var logger in Loggers)
            {
                logger.Info(string.Concat(type, ": "));
                logger.Info(@object == null
                                      ? "null"
                                      : @object.ToString());
            }
        }

        internal static void Error(object @object)
        {
            var type = new StackFrame(1).GetMethod().DeclaringType.Name;

            foreach (var logger in Loggers)
            {
                logger.Info(string.Concat(type, ": "));
                logger.Error(@object == null
                                      ? "null"
                                      : @object.ToString());
            }
        }
    }
}