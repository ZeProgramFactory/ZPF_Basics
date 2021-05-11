using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ZPF.SQL;

namespace ZPF.AT
{
   public class AuditTrailViewModel : BaseViewModel
   {
#if DEBUG
      //public static int MaxLines = 5000;
      public int MaxLines { get; set; } = 50000;
      public int MaxSize { get; set; } = 512 * 1024;
#else
      public int MaxLines { get; set; } = 500;
      public int MaxSize { get; set; } = 256 * 1024;
#endif

      public static string DateTimeFormat = "dd HHmmss";

      // - - -  - - - 

      /// <summary>
      /// AuditTrail.FKUser = FKUser
      /// </summary>
      public string FKUser
      {
         get { return _FKUser; }
         set { SetField(ref _FKUser, value, "FKUser"); }
      }

      private string _FKUser = "";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


      /// <summary>
      /// Init Globale pour un traitement
      /// Ex : AO_REF
      /// </summary>
      public string ItemID { get; set; }
      public string ItemType { get; set; }


      public string Application { get; set; }
      public string TerminalIP { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


      private static AuditTrailViewModel _Current = null;

      public static AuditTrailViewModel Current
      {
         get
         {
            if (_Current == null)
            {
               _Current = new AuditTrailViewModel();
            };

            return _Current;
         }

         set
         {
            _Current = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public AuditTrailViewModel()
      {
         if (_Current == null)
         {
            _Current = this;
         };

         Logs = new ObservableCollection<AuditTrail>();

         Logs.Add(new AuditTrail { Message = "Welcome to AT ..." });

         OnPropertyChanged("Logs");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private IAuditTrailWriter auditTrailWriter = null;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="messageWriter">AuditTrailWriter ... if null in memory tracing (Logs)</param>
      public void Init(IAuditTrailWriter messageWriter)
      {
         auditTrailWriter = messageWriter;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// complete last logs
      /// </summary>
      public ObservableCollection<AuditTrail> Logs { get => _Logs; set => SetField(ref _Logs, value); }
      private ObservableCollection<AuditTrail> _Logs = new ObservableCollection<AuditTrail>();

      /// <summary>
      /// filtered last logs
      /// </summary>
      public ObservableCollection<AuditTrail> AuditTrail { get => _AuditTrail; set => SetField(ref _AuditTrail, value); }

      private ObservableCollection<AuditTrail> _AuditTrail = new ObservableCollection<AuditTrail>();

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private AuditTrail _SelectedAuditTrail = null;

      public AuditTrail SelectedAuditTrail
      {
         get => _SelectedAuditTrail;
         set => SetField(ref _SelectedAuditTrail, value);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private string AuditTrailLevel2String(ErrorLevel Level)
      {
         switch (Level)
         {
            default:
            //case ErrorLevel.Debug: return "   ";
            case ErrorLevel.Log: return "*  ";
            case ErrorLevel.Info: return "** ";
            case ErrorLevel.Error: return "***";
            case ErrorLevel.Critical: return "!!!";
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public string FormatLine(AuditTrail message)
      {
         return string.Format("{0} {1} {2}", message.TimeStamp.ToString(DateTimeFormat), AuditTrailLevel2String(message.Level), message.Message);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void CompleteMessage(ref AuditTrail message)
      {
         message.FKUser = FKUser;
         if (message.TimeStampApp == DateTime.MinValue) message.TimeStampApp = DateTime.Now;
         if (string.IsNullOrEmpty(message.Application)) message.Application = AuditTrailViewModel.Current.Application;
         if (string.IsNullOrEmpty(message.TerminalID)) message.TerminalID = AuditTrailViewModel.Current.TerminalID;
         if (string.IsNullOrEmpty(message.TerminalIP)) message.TerminalID = AuditTrailViewModel.Current.TerminalIP;
         if (string.IsNullOrEmpty(message.ItemID)) message.ItemID = AuditTrailViewModel.Current.ItemID;
         if (string.IsNullOrEmpty(message.ItemType)) message.ItemType = AuditTrailViewModel.Current.ItemType;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void Write(AuditTrail message)
      {
         CompleteMessage(ref message);

         LogsAdd(message);

         if (auditTrailWriter != null)
         {
            auditTrailWriter.WriteLine(this, message);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public void LogsAdd(AuditTrail message)
      {
         if (Debugger.IsAttached)
         {
            Debug.WriteLine(string.Format("Enter LogsAddATVM" + message.Message));
            //   Debugger.Break();
         };

         CompleteMessage(ref message);

         // https://github.com/VOIP-PARTNERS/CallBooster/issues/118
         // https://appcenter.ms/orgs/VOIP-PARTNERS/apps/CallBooster-1/crashes/errors/2244476178u/overview
         if (Logs.Count > 0)
         {
            Logs.Insert(0, message);
         }
         else
         {
            Logs.Add(message);
         };

         OnPropertyChanged("Logs");

         if (Debugger.IsAttached)
         {
            Debug.WriteLine(string.Format("End LogsAddATVM" + message.Message));
            // Debugger.Break();
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public void Clean()
      {
         while (Logs.Count > MaxLines)
         {
            //Logs.RemoveAt(0);
            Logs.RemoveAt(Logs.Count - 1);
         }

         if (auditTrailWriter != null)
         {
            auditTrailWriter.Clean(this);
         }

         Write(new AuditTrail { Tag = "AuditTrail", Message = "Clean", IsBusiness = true });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
      public long Begin(AuditTrail message)
      {
         CompleteMessage(ref message);
         message.PK = -128;  // permet d'identifier les  begins en cours dans le monitoring
         message.Ticks = message.TimeStamp.Ticks;

         if (auditTrailWriter != null)
         {
            return auditTrailWriter.Begin(this, message);
         };

         return -1;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public void End(Int64 parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         if (auditTrailWriter != null)
         {
            auditTrailWriter.End(this, parent, errorLevel, message, dataOutType, dataOut);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      #region Filter

      private bool _ShowUser = true;
      public bool ShowUser { get => _ShowUser; set => _ShowUser = value; }
      public bool Childs { get; set; }
      public bool IsDebug { get; set; }
      public string UserWhere { get; set; }

      private ErrorLevel _Level = ErrorLevel.Info;
      public ErrorLevel Level
      {
         get { return _Level; }
         set { SetField(ref _Level, value); }
      }

      private string _TerminalID = "";
      public string TerminalID
      {
         get { return _TerminalID; }
         set { SetField(ref _TerminalID, value); }
      }

      // - - -  - - - 

      public string GetWhere(bool ShowUser = true)
      {
         string Where = "LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = UserAccount.PK WHERE 1=1 order by AuditTrail.PK desc";

         if (!ShowUser)
         {
            Where = "WHERE 1=1 order by AuditTrail.PK desc";
         };

         // - -  - -

         Where = Where.Replace("1=1", string.Format("Level>={0} and 1=1", (int)(Level)));

         if (!Childs)
         {
            Where = Where.Replace("1=1", "(Parent is null or Parent=0) and 1=1");
         };

         if (!IsDebug)
         {
            Where = Where.Replace("1=1", "IsBusiness = '1' and 1=1");
         };

         if (!string.IsNullOrEmpty(TerminalID))
         {
            Where = Where.Replace("1=1", string.Format("TerminalID = '{0}' and 1=1", TerminalID));
         };

         if (!string.IsNullOrEmpty(FKUser) && FKUser != "0")
         {
            Where = Where.Replace("1=1", string.Format("FKUser = '{0}' and 1=1", FKUser));
         };

         if (!string.IsNullOrEmpty(UserWhere) && FKUser != "0")
         {
            UserWhere = UserWhere.Trim();

            if (UserWhere.StartsWith("AND "))
            {
               Where = Where.Replace("1=1", $"{UserWhere.Substring(5)} and 1=1");
            }
            else if (UserWhere.StartsWith("OR "))
            {
               Where = Where.Replace("1=1", $"{UserWhere.Substring(4)} or 1=1");
            }
            else if (UserWhere.StartsWith("("))
            {
               if (!UserWhere.EndsWith(")"))
               {
                  UserWhere = UserWhere + ")";
               };

               Where = Where.Replace("1=1", $"{UserWhere} and 1=1");
            }
            else
            {
               Where = Where.Replace("1=1", $"{UserWhere} and 1=1");
            };
         };

         //if (!string.IsNullOrEmpty(Event) && Event != "01/01/1900 00:00:00")
         //{
         //   Where = Where.Replace("1=1", string.Format(" and ItemType like '{0}%' ", Event.Left(16)));
         //};

         return Where;
      }

      public string GetTerminalSQL()
      {
         string SQL = "SELECT ' ' AS Name, '' AS Value UNION SELECT DISTINCT TerminalID as Name, TerminalID as Value FROM AuditTrail ORDER BY Name";
         // ToDo: ¤¤¤ChM Optimize ces 2 requetes pour PG 2' sur grosse base

         return SQL;
      }

      public string GetUserSQL()
      {
         string SQL = "SELECT ' ' AS Name, '' AS Value UNION SELECT DISTINCT Login as Name, cast(UserAccount.PK as varchar(128)) as Value FROM AuditTrail LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = cast(UserAccount.PK as varchar(128))  where Login is not null ORDER BY Name ";

         return SQL;
      }

      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// Loads logs from file/db to property AuditTrail
      /// </summary>
      /// <param name="Filtered"></param>
      /// <param name="MaxRecords"></param>
      public void editTrail(bool Filtered = true, long MaxRecords = 500)
      {
         LoadAuditTrail(auditTrailWriter, Filtered, MaxRecords);
      }

      /// <summary>
      /// Loads logs from file/db to property AuditTrail
      /// </summary>
      /// <param name="messageWriter"></param>
      /// <param name="Filtered"></param>
      /// <param name="MaxRecords"></param>
      public void LoadAuditTrail(IAuditTrailWriter messageWriter, bool Filtered = true, long MaxRecords = 500)
      {
         if (auditTrailWriter != null)
         {
            AuditTrail = auditTrailWriter.LoadAuditTrail(this, Filtered, MaxRecords);
         };
      }

      /// <summary>
      /// Loads logs from file/db to property AuditTrail
      /// </summary>
      /// <param name="Filtered"></param>
      /// <param name="MaxRecords"></param>
      public void LoadAuditTrail(bool Filtered = true, long MaxRecords = 500)
      {
         if (auditTrailWriter != null)
         {
            AuditTrail = auditTrailWriter.LoadAuditTrail(this, Filtered, MaxRecords);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }

   // = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  =

   public static class Log
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      #region Write 

      public static void Write(AuditTrail auditTrail,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         auditTrail.Source = $"{sourceFilePath}({sourceLineNumber}) - {memberName}";

         AuditTrailViewModel.Current.Write(auditTrail);
      }

      public static void Write(string Tag, string Message,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         var source = $"{sourceFilePath}({sourceLineNumber}) - {memberName}";

         AuditTrailViewModel.Current.Write(new AuditTrail { Tag = Tag, Message = Message, Source = source });
      }

      public static void Write(ErrorLevel error, string Tag, string Message,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         var source = $"{sourceFilePath}({sourceLineNumber}) - {memberName}";

         AuditTrailViewModel.Current.Write(new AuditTrail { Level = error, Tag = Tag, Message = Message, Source = source });
      }

      public static void Write(ErrorLevel error, string Message,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         var source = $"{sourceFilePath}({sourceLineNumber}) - {memberName}";

         AuditTrailViewModel.Current.Write(new AuditTrail { Level = error, Message = Message, Source = source });
      }

      public static void Write(ErrorLevel error, Exception ex,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         AuditTrail at = new AuditTrail(ex, AuditTrail.TextFormat.TxtEx)
         {
            Level = error,
            Source = $"{sourceFilePath}({sourceLineNumber}) - {memberName}",
         };

         AuditTrailViewModel.Current.Write(at);
      }
      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static long Begin(AuditTrail auditTrail)
      {
         return AuditTrailViewModel.Current.Begin(auditTrail);
      }

      public static void End(long Parent, ErrorLevel errorLevel, string Message, string DataOutType = "", string DataOut = "")
      {
         AuditTrailViewModel.Current.End(Parent, errorLevel, Message, DataOutType, DataOut);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static void WriteHeader(string ProgName, string ProgVersion, string TerminalID)
      {
         Log.Write("", new string('-', 40));
         Log.Write("", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
         Log.Write("", string.Format("{0} {1}", ProgName, ProgVersion));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private static int StartStamp = System.Environment.TickCount;
      private static int LapStamp = System.Environment.TickCount;

      public static void InitTimeStamp()
      {
         StartStamp = System.Environment.TickCount;
         LapStamp = StartStamp;
      }

      public static string WriteTimeStamp(string Message)
      {
         int Stamp = System.Environment.TickCount;
         TimeSpan dt = TimeSpan.FromMilliseconds(Stamp - LapStamp);
         LapStamp = Stamp;

         AuditTrail ai = new AuditTrail
         {
            Message = string.Format("{0:00}:{1:00}:{2:00}.{3:000} {4}",
            dt.Hours, dt.Minutes, dt.Seconds, dt.Milliseconds,
            Message)
         };

         AuditTrailViewModel.Current.Write(ai);

         return ai.Message;
      }

      public static string WriteTotalTime(string Message)
      {
         int Stamp = System.Environment.TickCount;
         TimeSpan dt = TimeSpan.FromMilliseconds(Stamp - StartStamp);
         LapStamp = Stamp;

         AuditTrail ai = new AuditTrail
         {
            Message = string.Format("{0:00}:{1:00}:{2:00}.{3:000} {4}",
            dt.Hours, dt.Minutes, dt.Seconds, dt.Milliseconds,
            Message)
         };

         AuditTrailViewModel.Current.Write(ai);

         return ai.Message;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      #region MessageBox

      public static void MessageBox(ErrorLevel errorLevel, string Message)
      {
         MessageBox(errorLevel, Message, null);
      }

      public static void MessageBox(ErrorLevel errorLevel, string Message, string Data)
      {
         Write(new AuditTrail { Level = errorLevel, Tag = "", Message = Message, DataOut = Data });

         switch (errorLevel)
         {
            case ErrorLevel.Critical:
            case ErrorLevel.Error:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, Message);
               break;

            case ErrorLevel.Info:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Warning, Message);
               break;

            default:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Info, Message);
               break;
         };
      }

      public static void MessageBox(ErrorLevel errorLevel, Exception ex, string Tag = "")
      {
         AuditTrail at = new AuditTrail(ex) { Level = errorLevel, IsBusiness = true };
         Write(at);

         switch (errorLevel)
         {
            case ErrorLevel.Critical:
            case ErrorLevel.Error:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, at.Message);
               break;

            case ErrorLevel.Info:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Warning, at.Message);
               break;

            default:
               BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Info, at.Message);
               break;
         };
      }

      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}

