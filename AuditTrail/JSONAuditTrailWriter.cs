using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ZPF.AT
{
   public class JSONAuditTrailWriter : IAuditTrailWriter
   {
      private readonly string outputFile;

      public enum FileTypes { FullJSON, PartialJSON }
      private readonly FileTypes _FileType;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="outputFile"></param>
      /// <param name="DebugOutput">if or if not System.Diagnostics.Debug.WriteLine</param>
      public JSONAuditTrailWriter(string outputFile, FileTypes fileType = FileTypes.FullJSON)
      {
         this.outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
         this._FileType = fileType;
      }

      public void Clean(AuditTrailViewModel sender)
      {
         string json = "";

         if (System.IO.File.Exists(outputFile))
         {
            json = File.ReadAllText(outputFile);
         };

         if (_FileType == FileTypes.FullJSON)
         {
            // nothing to do
         }
         else
         {
            json = File.ReadAllText(outputFile);
            json = json + "}";
         };

         try
         {
            var Lines = JsonSerializer.Deserialize<List<AuditTrail>>(json);

            while (Lines.Count() > sender.MaxLines)
            {
               Lines.RemoveAt(0);
            };

            json = JsonSerializer.Serialize(Lines);

            File.WriteAllText(outputFile, json, System.Text.Encoding.ASCII);
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
         string json = "";

         if (System.IO.File.Exists(outputFile))
         {
            json = File.ReadAllText(outputFile);
         };


         if (_FileType == FileTypes.FullJSON)
         {
            // nothing to do
         }
         else
         {
            json = File.ReadAllText(outputFile);
            json = json + "}";
         };

         try
         {
            var lines = JsonSerializer.Deserialize<ObservableCollection<AuditTrail>>(json);

            //ToDo: filter

            while (lines.Count() > MaxRecords)
            {
               lines.RemoveAt(0);
            };

            return lines;
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);

            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
         };

         return sender.Logs;
      }

      public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
      {
         if (_FileType == FileTypes.FullJSON)
         {
            try
            {
               string json = "";

               if (System.IO.File.Exists(outputFile))
               {
                  json = File.ReadAllText(outputFile);
               };

               List<AuditTrail> lines = null;

               if (string.IsNullOrEmpty(json))
               {
                  lines = new List<AuditTrail>();
               }
               else
               {
                  lines = JsonSerializer.Deserialize<List<AuditTrail>>(json);
               };

               lines.Add(message);

               json = JsonSerializer.Serialize(lines, new JsonSerializerOptions { IgnoreNullValues = true });

               File.WriteAllText(outputFile, json, System.Text.Encoding.ASCII);
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
         else
         {
            try
            {
               string Line = JsonSerializer.Serialize(message, new JsonSerializerOptions
               {
                  IgnoreNullValues = true,
               });

               if (System.IO.File.Exists(outputFile))
               {
                  File.AppendAllLines(outputFile, new[] { "," + Line });
               }
               else
               {
                  File.AppendAllLines(outputFile, new[] { "{" + Line });
               };
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine("AuditTrailViewModel: " + ex.Message);
            };
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
