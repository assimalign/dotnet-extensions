using System;
using System.Diagnostics;
using System.Security;
using System.Runtime.Versioning;

namespace Assimalign.Extensions.Logging.EventLog
{
    [SupportedOSPlatform("windows")]
    internal sealed class WindowsEventLog : IEventLog
    {
        // https://msdn.microsoft.com/EN-US/library/windows/desktop/aa363679.aspx
        private const int MaximumMessageSize = 31839;
        private bool _enabled = true;

        public WindowsEventLog(string logName, string machineName, string sourceName)
        {
            DiagnosticsEventLog = new EventLog(logName, machineName, sourceName);
        }

        public EventLog DiagnosticsEventLog { get; }

        public int MaxMessageSize => MaximumMessageSize;

        public int? DefaultEventId { get; set; }

        public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
        {
            try
            {
                if (_enabled)
                {
                    DiagnosticsEventLog.WriteEvent(new EventInstance(eventID, category, type), message);
                }
            }
            catch (SecurityException sx)
            {
                _enabled = false;
                // We couldn't create the log or source name. Disable logging.
                try
                {
                    using (var backupLog = new EventLog("Application", ".", "Application"))
                    {
                        backupLog.WriteEvent(new EventInstance(instanceId: 0, categoryId: 0, EventLogEntryType.Error),
                            $"Unable to log .NET application events. {sx.Message}");
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
