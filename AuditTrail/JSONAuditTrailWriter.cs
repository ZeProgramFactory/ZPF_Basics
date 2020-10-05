using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ZPF.AT
{
   public class JSONAuditTrailWriter : IAuditTrailWriter
   {
      private readonly string outputFile;
      private readonly bool DebugOutput;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="outputFile"></param>
      /// <param name="DebugOutput">if or if not System.Diagnostics.Debug.WriteLine</param>
      public JSONAuditTrailWriter(string outputFile, bool DebugOutput = true)
      {
         this.outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
         this.DebugOutput = DebugOutput;
      }

      public void Clean(AuditTrailViewModel sender)
      {
         try
         {
            TStrings Lines = new TStrings();

            Lines.LoadFromFile(outputFile);

            while (Lines.Count > sender.MaxLines)
            {
               Lines.Delete(0);
            };

            Lines.SaveToFile(outputFile, System.Text.Encoding.ASCII);
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);

            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
         };
      }

      public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         return sender.Logs;
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         try
         {
            string Line = sender.FormatLine(message);

            if (DebugOutput) System.Diagnostics.Debug.WriteLine(Line);

            File.AppendAllLines(outputFile, new[] { Line });

            if ((message.Level == ErrorLevel.Critical) && (string.IsNullOrEmpty(message.DataOut)))
            {
               File.AppendAllLines(outputFile, new[] { message.DataOut + Environment.NewLine });
            };
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine("AuditTrailViewModel: " + ex.Message);
         };
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
