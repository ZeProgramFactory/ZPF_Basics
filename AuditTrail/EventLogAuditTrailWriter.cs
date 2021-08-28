using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ZPF.AT
{
   public class EventLogAuditTrailWriter : IAuditTrailWriter
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="outputFile"></param>
      /// <param name="DebugOutput">if or if not System.Diagnostics.Debug.WriteLine</param>
      public EventLogAuditTrailWriter()
      {
      }

      public void Clean(AuditTrailViewModel sender)
      {
      }

      public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         return sender.Logs;
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         using (EventLog eventLog = new EventLog("Application"))
         {
            eventLog.Source = "Syscall";

            EventLogEntryType eventLogEntryType = EventLogEntryType.Error;

            switch (message.Level)
            {
               case ErrorLevel.Log:
                  eventLogEntryType = EventLogEntryType.Information;
                  break;

               case ErrorLevel.Info:
                  eventLogEntryType = EventLogEntryType.Warning;
                  break;

               case ErrorLevel.Error:
                  eventLogEntryType = EventLogEntryType.Error;
                  break;

               case ErrorLevel.Critical:
                  eventLogEntryType = EventLogEntryType.Error;
                  break;
            }

            if (eventLogEntryType == EventLogEntryType.Error)
            {
               //eventLog.WriteEntry(message.Message, eventLogEntryType, 0815, 4711, null);
               eventLog.WriteEntry(message.Message, eventLogEntryType);
            };
         }

      }

      public long Begin(AuditTrailViewModel sender, AuditTrail message)
      {
         WriteLine(sender, message);
         return 1;
      }

      public void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         AuditTrail at = new AuditTrail { Level = errorLevel, Message = message, DataOutType = dataOutType, DataOut = dataOut };
         WriteLine(sender, at);
      }
   }
}
