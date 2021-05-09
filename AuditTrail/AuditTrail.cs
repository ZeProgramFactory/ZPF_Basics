using System;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Serialization;
using ZPF;
using ZPF.SQL;

namespace ZPF.AT
{
   public enum ErrorLevel { Log = 0, Info = 1, Error = 2, Critical = 3 }

   public class AuditTrail
   {
      #region SQLCreate

      public static readonly string PostScript_SQLite = @"
CREATE VIEW [AuditTrail_V_Last100]
AS
SELECT PK, TimeStamp, [Level], Tag, Message, DataIn, DataOut, TerminalID, FKUser, ItemID, ItemType
FROM   AuditTrail
ORDER BY PK DESC
limit 100;
";

      public static readonly string PostScript_MSSQL = @"
EXECUTE('CREATE VIEW [AuditTrail_V_Last100]
AS
SELECT TOP (100) PK, TimeStamp, [Level], Tag, Application, Message, DataIn, DataOut, TerminalID, FKUser, ItemID, ItemType
FROM   AuditTrail
ORDER BY PK DESC;');
";

      public static readonly string PostScript_MySQL = @"
CREATE VIEW `AuditTrail_V_Last100` AS 
select 
   `audittrail`.`PK` AS `PK`,`audittrail`.`TimeStamp` AS `TimeStamp`,`audittrail`.`Level` AS `Level`,`audittrail`.`Tag` AS `Tag`, Application, `audittrail`.`Message` AS `Message`,`audittrail`.`DataIn` AS `DataIn`,`audittrail`.`DataOut` AS `DataOut`,`audittrail`.`TerminalID` AS `TerminalID`,`audittrail`.`FKUser` AS `FKUser`,`audittrail`.`ItemID` AS `ItemID`,`audittrail`.`ItemType` AS `ItemType` 
from 
   `audittrail` 
order by 
   `audittrail`.`PK` desc 
limit 100 ;";

      public static readonly string PostScript_PGSQL = @"
CREATE OR REPLACE VIEW public.audittrail_v_last100
AS
SELECT audittrail.pk, audittrail.""timestamp"", audittrail.level, audittrail.tag, audittrail.application, audittrail.message,
audittrail.datain, audittrail.dataout, audittrail.terminalid, audittrail.fkuser, audittrail.itemid, audittrail.itemtype
FROM audittrail
ORDER BY audittrail.pk DESC
LIMIT 100;
";

      #endregion

      // - - -  - - - 

      public static TStrings Dico = TStrings.FromJSon("[{\"1\":\"Log\"}, {\"2\":\"Info\"}, {\"3\":\"Error\"}, {\"4\":\"Critical\"} ]");
      public enum TextFormat { Txt = 0, TxtEx = 1, HTML = 2 }

      // - - -  - - - 

      #region Creators 

      public AuditTrail()
      {
         TimeStamp = DateTime.Now;
         Level = ErrorLevel.Log;
         Tag = "";
      }

      public AuditTrail(Exception ex, TextFormat textFormat = TextFormat.Txt)
      {
         TimeStamp = DateTime.Now;

         Level = ErrorLevel.Critical;
         Tag = "Exception";

         if (ex != null)
         {
            Message = ex.Message;

            switch (textFormat)
            {
               default:
               case TextFormat.Txt:
                  DataOutType = "TXT";
                  DataOut = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine + ex.Source;
                  break;

               case TextFormat.TxtEx:
                  DataOutType = "TXTEx";

                  DataOut = "!" + ex.Message + Environment.NewLine
                  + Environment.NewLine
                  + "#StackTrace" + Environment.NewLine
                  + ex.StackTrace + Environment.NewLine
                  + Environment.NewLine
                  + "#Source" + Environment.NewLine
                  + ex.Source;
                  break;

               case TextFormat.HTML:
                  DataOutType = "HTML";

                  TStrings html = new TStrings();

                  html.Add("<p style='font-family: Consolas,monospace; bgcolor=transparent;'>");
                  html.Add(string.Format("{0}</br>", "Message"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.Message));

                  html.Add(string.Format("{0}</br>", "StackTrace"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.StackTrace));

                  html.Add(string.Format("{0}</br>", "Source"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.Source));
                  html.Add("</p>");

                  DataOut = html.Text;
                  break;
            };
         };
      }

      #endregion

      // - - -  - - - 

      [DB_Attributes.PrimaryKey(true)]
      public Int64 PK { get; set; }

      [DB_Attributes.Index()]
      public DateTime TimeStamp { get; set; }
      [JsonIgnore]
      public DateTime TimeStampApp { get; set; }
      [JsonIgnore]
      public DateTime TimeStampDB { get; set; }

      [DB_Attributes.Index()]
      public ErrorLevel Level { get; set; }

      [JsonIgnore]
      [DB_Attributes.Ignore]
      public string sLevel
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return Dico["1"];
               case ErrorLevel.Info: return Dico["2"];
               case ErrorLevel.Error: return Dico["3"];
               case ErrorLevel.Critical: return Dico["4"];
               default: return Dico["4"];
            };
         }
      }

      public Int64 Parent { get; set; }
      public bool IsBusiness { get; set; }

      [DB_Attributes.Index()]
      [DB_Attributes.MaxLength(32)]
      public string Tag { get; set; }

      [DB_Attributes.Index()]
      [DB_Attributes.MaxLength(128)]
      public string Application { get; set; }

      [DB_Attributes.MaxLength(512)]
      public string Message { get; set; }

      public Int64 Ticks { get; set; }

      [JsonIgnore]
      [DB_Attributes.Ignore]
      public TimeSpan Duration
      {
         get => new TimeSpan(Ticks);
         set => Ticks = value.Ticks;
      }

      [DB_Attributes.MaxLength(-1)]
      public string DataIn { get; set; }
      [DB_Attributes.MaxLength(32)]
      public string DataInType { get; set; }

      [DB_Attributes.MaxLength(-1)]
      public string DataOut { get; set; }
      [DB_Attributes.MaxLength(32)]
      public string DataOutType { get; set; }

      [DB_Attributes.MaxLength(128)]
      public string TerminalID { get; set; }

      [DB_Attributes.MaxLength(128)]
      public string TerminalIP { get; set; }

      [DB_Attributes.MaxLength(128)]
      public string FKUser { get; set; }

      [DB_Attributes.MaxLength(128)]
      public string ItemID { get; set; }

      [DB_Attributes.MaxLength(128)]
      public string ItemType { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      [JsonIgnore]
      [DB_Attributes.Ignore]
      public bool HasData { get { return !string.IsNullOrEmpty(DataIn) || !string.IsNullOrEmpty(DataOut); } }

      // - - -  - - - 

      [JsonIgnore]
      [DB_Attributes.Ignore]
      public string Foreground
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return (Ticks > 0 ? "#FF000000" /*Brushes.Black*/ : "#FF708090" /*Brushes.SlateGray*/);
               case ErrorLevel.Info: return (Ticks > 0 ? "#FF000000" /*Brushes.Black*/ : "#FFFFFFFF" /*Brushes.White*/);
               default: return "#FFFFFFFF" /*Brushes.White*/;
            };
         }
      }

      [JsonIgnore]
      [DB_Attributes.Ignore]
      public string Background
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return (Ticks > 0 ? "#FF008000" /*Brushes.Green*/ : "#FFF5F5F5" /*Brushes.WhiteSmoke*/);
               case ErrorLevel.Info: return (Ticks > 0 ? "#FF008000" /*Brushes.Green*/ : "#FFF5DEB3" /*Brushes.Wheat*/);
               case ErrorLevel.Error: return "#FFFA8072" /*Brushes.Salmon*/;
               case ErrorLevel.Critical: return "#FF8B0000" /*Brushes.DarkRed*/;
            };

            return "#FF800080" /*Brushes.Purple*/;
         }
      }

      // - - -  - - -

      public static AuditTrail FromHere(ErrorLevel errorLevel, string tag, string message)
      {
         TStrings st = new TStrings();
         st.Text = Environment.StackTrace;

         var data = st[2];
         if (string.IsNullOrEmpty(data))
         {
            data = Environment.StackTrace;
         };

         return new AuditTrail
         {
            Level = errorLevel,
            Tag = tag,
            Message = message,
            DataInType = "TXT",
            DataIn = data,
         };
      }

      public static AuditTrail WithStack(ErrorLevel errorLevel, string tag, string message)
      {
         TStrings st = new TStrings();
         st.Text = Environment.StackTrace;

         if (st.Count > 3)
         {
            st.Delete(0);
            st.Delete(0);
         };

         return new AuditTrail
         {
            Level = errorLevel,
            Tag = tag,
            Message = message,
            DataInType = "TXT",
            DataIn = st.Text,
         };
      }

      // - - -  - - -

      public override string ToString()
      {
         return $"{TimeStamp.ToString("HH:mm:ss")} { (new String('*', (int)Level) + "----").Left(4) } {Tag.Left(10)} {Message}";
      }
   }
}
