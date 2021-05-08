using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZPF.SQL;

namespace ZPF.AT
{
   public class DBAuditTrailWriter : IAuditTrailWriter
   {
      DBSQLViewModel _dBSQLViewModel = null;
      private readonly bool DebugOutput;

      public DBAuditTrailWriter(DBSQLViewModel dBSQLViewModel, bool DebugOutput = true)
      {
         _dBSQLViewModel = dBSQLViewModel;
         AuditTrailWriterHelper.CreateTable(_dBSQLViewModel);
         this.DebugOutput = DebugOutput;
      }

      public void Clean(AuditTrailViewModel sender)
      {
         throw new NotImplementedException();
      }

      private string GetWhere(AuditTrailViewModel sender, bool ShowUser = true)
      {
         return AuditTrailWriterHelper.GetWhere(_dBSQLViewModel.DBType, sender, ShowUser);
      }


      public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         return AuditTrailWriterHelper.LoadAuditTrail(_dBSQLViewModel, sender, Filtered, MaxRecords);
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         //AuditTrailWriterHelper.LogsAdd(sender, message);

         AuditTrailWriterHelper.WriteLineDB(_dBSQLViewModel, sender, message);
      }

      public long Begin(AuditTrailViewModel sender, AuditTrail message)
      {

         //AuditTrailWriterHelper.LogsAdd(sender, message);

         return AuditTrailWriterHelper.BeginWriteDB(_dBSQLViewModel, sender, message);
          
      }

      public void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         AuditTrailWriterHelper.EndWriteDB(_dBSQLViewModel, parent, errorLevel, message, dataOutType, dataOut);
      }

   }
}
