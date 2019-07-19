using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZPF.AT
{
   public class DebugAuditTrailWriter : IAuditTrailWriter
   {
      public void Clean(AuditTrailViewModel sender)
      {
         // nothing to do ...
      }

      public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         return sender.Logs;
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         System.Diagnostics.Debug.WriteLine(message.Message);
         sender.LogsAdd(message);
      }

      public long Begin(AuditTrailViewModel sender, AuditTrail message)
      {
         WriteLine(sender, message);
         return -1;
      }

      public void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         return;
      }

   }
}
