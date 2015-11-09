﻿namespace WindowsServiceHosting.Loggers
{
    using System.Diagnostics;

    public class WindowsEventLogger : ILogger
    {
        public void AddMessage(string message)
        {
            using (var log = new EventLog("Application"))
            {
                log.Source = "NortwindWCFServiceHostLog";
                log.WriteEntry(message, EventLogEntryType.Information);
            }
        }

        public void AddError(string errorMessage)
        {
            using (var log = new EventLog("Application"))
            {
                log.Source = "NortwindWCFServiceHostLog";
                log.WriteEntry(errorMessage, EventLogEntryType.Error);
            }
        }
    }
}