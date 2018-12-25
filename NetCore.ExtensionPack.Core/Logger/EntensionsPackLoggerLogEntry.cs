using System;

namespace ExtensionsPack.Core.Logger
{
    public class EntensionsPackLoggerLogEntry
    {
        public string Message { get; }
        public string Severity { get; }
        public Exception Exception { get; }

        public EntensionsPackLoggerLogEntry(Exception exception, string severity, string message)
        {
            this.Exception = exception;
            this.Message = message;
            this.Severity = severity;
        }
    }
}
