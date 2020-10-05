using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZPF.SQL;

namespace ZPF.AT
{
   public class DBAndFileAuditTrailWriter : IAuditTrailWriter
   {
      DBSQLViewModel _dBSQLViewModel = null;
      FileAuditTrailWriter fileAuditTrailWriter = null;

      private readonly string outputFile;
      private readonly bool DebugOutput;
      private readonly bool IsDB = false;

      public DBAndFileAuditTrailWriter(DBSQLViewModel dBSQLViewModel, string outputFile, bool DebugOutput = true)
      {
         _dBSQLViewModel = dBSQLViewModel;
         IsDB = AuditTrailWriterHelper.CreateTable(_dBSQLViewModel);

         if (!string.IsNullOrEmpty(outputFile))
         {
            fileAuditTrailWriter = new FileAuditTrailWriter(outputFile, DebugOutput);
         };

         this.outputFile = outputFile ?? null;
         this.DebugOutput = DebugOutput;
      }

      public void Clean(AuditTrailViewModel sender)
      {
         //AuditTrailWriterHelper.Clean(sender, outputFile);
      }

      private string GetWhere(AuditTrailViewModel sender, bool ShowUser = true)
      {
         return AuditTrailWriterHelper.GetWhere(sender, ShowUser);
      }


      public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         return AuditTrailWriterHelper.LoadAuditTrail(_dBSQLViewModel, sender, Filtered, MaxRecords);
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         fileAuditTrailWriter?.WriteLine(sender, message);
         //AuditTrailWriterHelper.LogsAdd(sender, message);

         //// File in first ( Wanao Specs ...)
         //AuditTrailWriterHelper.WriteLineFile(sender, message, this.outputFile, DebugOutput);

         AuditTrailWriterHelper.WriteLineDB(_dBSQLViewModel, sender, message);
      }

      public long Begin(AuditTrailViewModel sender, AuditTrail message)
      {
         fileAuditTrailWriter?.Begin(sender, message);
         //AuditTrailWriterHelper.LogsAdd(sender, message);
         //AuditTrailWriterHelper.WriteLineFile(sender, message, this.outputFile, DebugOutput);

         return AuditTrailWriterHelper.BeginWriteDB(_dBSQLViewModel, sender, message);
      }

      public void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         AuditTrail at = new AuditTrail { Level = errorLevel, Message = message, DataOutType = dataOutType, DataOut = dataOut };

         fileAuditTrailWriter?.End(sender, parent, errorLevel, message, dataOutType, dataOut);

         //AuditTrailWriterHelper.WriteLineFile(sender, at, this.outputFile, DebugOutput);
         //// ?? AuditTrailWriterHelper.LogsAdd(sender, message);

         AuditTrailWriterHelper.EndWriteDB(_dBSQLViewModel, parent, errorLevel, message, dataOutType, dataOut);
      }
   }
}
