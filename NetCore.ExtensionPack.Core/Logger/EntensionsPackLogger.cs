using System;

namespace ExtensionsPack.Core.Logger
{
    public static class EntensionsPackLogger
    {
        private static Action<EntensionsPackLoggerLogEntry> OnLogEntryAdded { get; set; }

        public static void AttachLogger(Action<EntensionsPackLoggerLogEntry> handler)
        {
            OnLogEntryAdded += handler;
        }

        public static void DetachAllLoggers()
        {
            OnLogEntryAdded = null;
        }

        internal static void Info(string message)
        {
            OnLogEntryAdded?.Invoke(new EntensionsPackLoggerLogEntry(null, "INFO", message));
        }

        internal static void Debug(string message)
        {
            OnLogEntryAdded?.Invoke(new EntensionsPackLoggerLogEntry(null, "DEBUG", message));
        }

        internal static void Warn(string message)
        {
            OnLogEntryAdded?.Invoke(new EntensionsPackLoggerLogEntry(null, "WARN", message));
        }

        internal static void Error(Exception ex)
        {
            Error(ex, ex.Message);
        }

        internal static void Error(Exception ex, string message)
        {
            OnLogEntryAdded?.Invoke(new EntensionsPackLoggerLogEntry(ex, "ERROR", message));
        }
    }
}
