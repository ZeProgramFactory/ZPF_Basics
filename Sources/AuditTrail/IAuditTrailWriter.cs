using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ZPF.AT
{
   public interface IAuditTrailWriter
   {
      void WriteLine(AuditTrailViewModel sender, AuditTrail message);

      void Clean(AuditTrailViewModel sender);

      long Begin(AuditTrailViewModel sender, AuditTrail message);

      void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut);

      ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500);
   }
}
