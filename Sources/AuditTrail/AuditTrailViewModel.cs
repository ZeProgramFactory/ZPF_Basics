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

         //Logs = new List<AuditTrail>();
         Logs = new ObservableCollection<AuditTrail>();

         Logs.Add(new AuditTrail { Message = "Welcome to AT ..." });
         OnPropertyChanged("Logs");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private IAuditTrailWriter auditTrailWriter = null;

      public void Init(IAuditTrailWriter messageWriter)
      {
         auditTrailWriter = messageWriter;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// complete last logs
      /// </summary>
      //public List<AuditTrail> Logs { get => _Logs; set => SetField(ref _Logs, value); }
      //private List<AuditTrail> _Logs = new List<AuditTrail>();

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
         //message.FKUser = FKUser;
         CompleteMessage(ref message);

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
         Logs.Add(message);
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
            Logs.RemoveAt(0);
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

      // ToDo: ¤¤¤ChM Optimize ces 2 requetes pour PG 2' sur grosse base
      public string GetTerminalSQL()
      {
         string SQL = "SELECT ' ' AS Name, '' AS Value UNION SELECT DISTINCT TerminalID as Name, TerminalID as Value FROM AuditTrail ORDER BY Name";

         return SQL;
      }

      public string GetUserSQL()
      {
         string SQL = "SELECT ' ' AS Name, '' AS Value UNION SELECT DISTINCT Login as Name, cast(UserAccount.PK as varchar(128)) as Value FROM AuditTrail LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = cast(UserAccount.PK as varchar(128))  where Login is not null ORDER BY Name ";
         // ToDo :  ChM remplace String SQL = "SELECT ' ' AS Name, '' AS Value UNION SELECT DISTINCT Login as Name, UserAccount.PK as Value FROM AuditTrail LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = UserAccount.PK where Login is not null ORDER BY Name ";
         // testé sur SQLServer; SQLite; PG

         return SQL;
      }

      //public DataSet GetAudittrailList()
      //{
      //   String SQL = GetAudittrailListSQL(_Connection);

      //   DataTable dt = DB_SQL.QuickQueryView(_Connection, SQL) as DataTable;

      //   return DataTable2DataSet(dt);
      //}

#if NETSTANDARD2_0
      //ZPF_Basics V Git
      //private string GetAudittrailListSQL(DBType dbType)
      //{
      //   string Where = GetWhere(true);
      //   string SQL = ZPF.SQL.DB_SQL.SelectAll(dbType, "AuditTrail", Where, 500);

      //   return SQL;
      //}
#endif

      //public DataSet GetTerminalList()
      //{
      //   String SQL = GetTerminalSQL();

      //   DataTable dt = DB_SQL.QuickQueryView(_Connection, SQL) as DataTable;

      //   return DataTable2DataSet(dt);
      //}

      //      public DataSet GetEventList()
      //      {
      //         String SQL = @"
      //SELECT ' ' AS Name, '' AS Value, 1 as SortOrder
      //UNION 
      //SELECT Distinct (Tag + ' ' + ItemType) as Name, CONVERT(datetime, ItemType, 104) as Value, 2 as SortOrder FROM AuditTrail where ItemType is not null 
      //order by SortOrder, Value desc
      //";

      //         DataTable dt = DB_SQL.QuickQueryView(_Connection, SQL) as DataTable;

      //         return DataTable2DataSet(dt);
      //      }

      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

#if !PCL && !NETCOREAPP1_0
      public void LoadAuditTrail(bool Filtered = true, long MaxRecords = 500)
      {
         LoadAuditTrail(auditTrailWriter, Filtered, MaxRecords);
      }

      public void LoadAuditTrail(IAuditTrailWriter messageWriter, bool Filtered = true, long MaxRecords = 500)
      {
         if (auditTrailWriter != null)
         {
            AuditTrail = auditTrailWriter.LoadAuditTrail(this, Filtered, MaxRecords);
         };
      }
#endif

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }

   // = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  = =  =

   public static class Log
   {
      public static void Debug(ErrorLevel error, string Message)
      {
         AuditTrailViewModel.Current.Write(new AuditTrail { IsBusiness = false, Level = error, Message = Message });
      }

      public static void Debug(string Tag, string Message)
      {
         AuditTrailViewModel.Current.Write(new AuditTrail { IsBusiness = false, Tag = Tag, Message = Message });
      }

      public static void Debug(ErrorLevel errorLevel, string Tag, Exception ex, string Data)
      {
         var at = new AuditTrail(ex, AuditTrail.TextFormat.TxtEx)
         {
            Level = errorLevel,
            Tag = Tag,
            IsBusiness = false,
         };

         at.DataOut = (string.IsNullOrEmpty(Data) ? at.DataOut : Data + Environment.NewLine + Environment.NewLine + at.DataOut);

         AuditTrailViewModel.Current.Write(at);
      }

      public static void Debug(ErrorLevel error, Exception ex,
                                    [CallerMemberName] string memberName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0)
      {
         AuditTrail at = new AuditTrail(ex, AuditTrail.TextFormat.TxtEx)
         {
            Level = error,
            IsBusiness = false,
         };

         AuditTrailViewModel.Current.Write(at);
      }

      public static void Debug(AuditTrail auditTrail)
      {
         auditTrail.IsBusiness = false;
         AuditTrailViewModel.Current.Write(auditTrail);
      }

      public static void Write(AuditTrail auditTrail)
      {
         auditTrail.IsBusiness = true;
         AuditTrailViewModel.Current.Write(auditTrail);
      }

      public static void Write(string Tag, string Message)
      {
         AuditTrailViewModel.Current.Write(new AuditTrail { IsBusiness = true, Tag = Tag, Message = Message });
      }

      public static void Write(ErrorLevel error, string Message)
      {
         AuditTrailViewModel.Current.Write(new AuditTrail { IsBusiness = true, Level = error, Message = Message });
      }

      public static void Write(ErrorLevel error, Exception ex,
                                 [CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0)
      {
         AuditTrail at = new AuditTrail(ex, AuditTrail.TextFormat.TxtEx)
         {
            Level = error,
            IsBusiness = true,
         };

         AuditTrailViewModel.Current.Write(at);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
      //ToDo : ChM Begin End 
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -



      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


      public static long Begin(AuditTrail auditTrail)
      {
         auditTrail.IsBusiness = true;
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
         AuditTrail at = new AuditTrail(ex) { Level = errorLevel, IsBusiness = false };
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

/*

public class AuditTrailViewModel : BaseViewModel
{

#if !PCL
   private DbConnection _Connection = null;
   private string _Application = null;
   /// <summary>
   /// _TerminalIP  init Wanao : System.Environment.UserName
   /// </summary>
   private string _TerminalIP = null;

   public DbConnection Connection
   {
      get { return _Connection; }
      private set { SetField(ref _Connection, value); }
   }
#endif

   bool _IsDBInit = false;
   public bool IsDBInit { get => _IsDBInit; set => _IsDBInit = value; }


#if PCL  && SQLLITE
   private SQLite.Net.SQLiteConnection _Connection = null;

   public SQLite.Net.SQLiteConnection Connection
   {
      get { return _Connection; }
      set
      {
         SetField(ref _Connection, value);
         IsDBInit = _Connection != null;
      }
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

#if !PCL
   public void Init(DbConnection dbConnection, string Application = "", string TerminalIP="")
   {
      _Connection = dbConnection;
      _Application = Application;
      _TerminalIP = TerminalIP;

      IsDBInit = _Connection != null;

      if (_Connection != null)
      {
         CreateTable();
         InitDB();
      };
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static AuditTrailViewModel _Instance = null;

   public static AuditTrailViewModel Instance
   {
      get
      {
         if (_Instance == null)
         {
            _Instance = new AuditTrailViewModel();
         };

         return _Instance;
      }

      set
      {
         _Instance = value;
      }
   }

   public AuditTrailViewModel()
   {
      if (_Instance == null) _Instance = this;

#if !PCL
      _Connection = DB_SQL.Connection;
#endif

      FileName = "AuditTrail.txt";

      AuditTrail = new List<AuditTrailList>();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static AuditTrailViewModel _Secondary = null;

   public static AuditTrailViewModel Secondary
   {
      get
      {
         if (_Secondary == null)
         {
            _Secondary = new AuditTrailViewModel();
         };

         return _Secondary;
      }

      set
      {
         _Secondary = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public void InitDB()
   {
#if SqlServer || MySQL || PostgreSQL
      if (_Connection != null)
      {
         CreateTable();
      };
#endif

#if SQLLITE
      if (DB_SQL.SQLiteConnection != null)
      {
         DB_SQL.SQLiteConnection.CreateTable<AuditTrail>();
      };
#endif
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public TStrings GetLastLines(int NbLines = 10)
   {
      TStrings Result = new TStrings();

      foreach (var l in AuditTrail)
      {
         Result.Add(string.Format("{0} {1} {2} {3} {4} ", l.TimeStamp.ToString("dd HH:mm"), l.Level.ToString(), l.Tag, l.Message, l.DataOut));
      };

      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

#if (SqlServer || MySQL || PostgreSQL || SQLLITE) && ! XF 
   public bool CreateTable(DbConnection dbConnection = null)
   {
      bool Result = true;

      if (dbConnection == null) dbConnection = _Connection;

      DB_SQL.QuickQueryInt(dbConnection, "select PK from AuditTrail where 1=2");
      if (DB_SQL._ViewModel.LastError == "")
      {
         return false;
      };

      if (Result)
      {
         TStrings SQL = new TStrings();

         if (dbConnection.GetType().ToString().Contains("SqlConnection"))
         {
            SQL.Text = AuditTrail.SQLCreate_MSSQL;
         };

         if (dbConnection.GetType().ToString().Contains("Npgsql"))
         {
            SQL.Text = AuditTrail.SQLCreate_PGSQL;
         };

         if (dbConnection.GetType().ToString().Contains("MySqlConnection"))
         {
            SQL.Text = AuditTrail.SQLCreate_MYSQL;
         };

         if (_Connection.DBType == DB_SQL.DBType.SQLite )
         {
            SQL.Text = AuditTrail.SQLCreate_SQLite;
         };

#if !NETCOREAPP1_0 && ! XF
         Result = DB_SQL.RunScript(dbConnection, "", SQL);
#else
         Result = false;
#endif
         if (!Result && dbConnection == null)
         {
            this.Write(new AuditTrail() { Level = ErrorLevel.Error, Message = DB_SQL._ViewModel.LastError + Environment.NewLine + DB_SQL._ViewModel.LastQuery });

            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };

            return false;
         }
      };

      return Result;
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -


   private DataSet DataTable2DataSet(DataTable dataTable)
   {
      DataSet Result = new DataSet();
      Result.Tables.Add(dataTable);
      return Result;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public List<AuditTrailList> AuditTrail { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   Func<MessageBoxType, string, string, bool> _MsgCallBack = null;

   public void InitMsgCallBack(Func<MessageBoxType, string, string, bool> MsgCallBack)
   {
      _MsgCallBack = MsgCallBack;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public string FileName { get; set; }

   /// <summary>
   /// Init Globale pour un traitement
   /// Ex : AO_REF
   /// </summary>
   public string ItemID { get; set; }
   public string ItemType { get; set; }


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

#if PCL
   public async Task WriteTXT(AuditTrail auditTrail)
   {
      AuditTrail.Add(auditTrail.Copy2AuditTrailList());
      await AddToFile(auditTrail);
   }

   public async Task Write(AuditTrail auditTrail)
   {
      AuditTrail.Add(auditTrail.Copy2AuditTrailList());
      await AddToFile(auditTrail);

#if SQLLITE
      if (_Connection != null)
      {
         StringToSQL(ref auditTrail); 
         DB_SQL.Insert(auditTrail);
      };
#endif
   }

#else

   public delegate void CallBackHandler(ref AuditTrail auditTrail, ref bool Handled);
   public event CallBackHandler CallBack;

   public bool Write(AuditTrail auditTrail)
   {
      return DoWrite(auditTrail);
   }

   public Int64 Begin(AuditTrail auditTrail)
   {
      if (DoWrite(auditTrail))
      {
         //if (DB_SQL.Check(_Connection, "AuditTrail", auditTrail))
         {
            //public static bool Check(DbConnection dbConnection, string TableName, Object obj)
            //{
            TStrings PK = ObjToRecord(_Connection, auditTrail, ObjToRecordActionType.ExcludePK);

            string Where = "";

            for (int i = 0; i < PK.Count; i++)
            {
               Where = Where + PK[i].ReplaceLastOccurrence(",", " and ");
            }
            Where = Where.Substring(0, Where.Length - 5);

            //   Where = Where.Substring(0, Where.Length - 1).Replace(",", " and ");  //  replace remplace toutes les occurences meme dans les fonctions sql

            string SQL = String.Format("select PK from {0} where {1} order by PK desc", "AuditTrail", Where);

            return DB_SQL.QuickQueryInt64(_Connection, SQL);
            //if (Res != null)
            //{
            //   return (Int64)Res;
            //}
            //else
            //{
            //   return -2; 
            //}
         };
      }
      else
      {
         //¤
         //  DoWrite(new AuditTrail() { Tag = "Debug Log.Begin", Message = "DoWrite KO DB_SQL._ViewModel.LastError :" + DB_SQL._ViewModel.LastError });
         return -1;
      };
   }

   public bool End(Int64 parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
   {
      long Ticks = DateTime.Now.Ticks;

      var at = DB_SQL.QueryFirst<AuditTrail>(_Connection, "select * from AuditTrail where PK=" + parent);

      if (at != null)
      {
         at.Level = errorLevel;
         at.Ticks = Ticks - at.Ticks;
         at.DataOutType = dataOutType;
         at.DataOut = dataOut;

         var endAT = new AuditTrail();
         endAT.Application = at.Application;
         endAT.TerminalID = at.TerminalID;
         endAT.TerminalIP = at.TerminalIP;
         endAT.FKUser = at.FKUser;
         endAT.ItemID = at.ItemID;
         endAT.ItemType = at.ItemType;
         endAT.IsBusiness = at.IsBusiness;
         endAT.Tag = at.Tag;
         endAT.Parent = parent;
         endAT.Message = message;
         endAT.Level = errorLevel;
         endAT.DataOutType = dataOutType;
         endAT.DataOut = dataOut;

         StringToSQL(ref endAT);
         DB_SQL.Insert(_Connection, endAT);

         StringToSQL(ref at);
         return DB_SQL.Update(_Connection, at);
      }
      else
      {
         return false;
      };
   }

   public bool DoWrite(AuditTrail auditTrail)
   {
      if (auditTrail == null)
      {
         return false;
      };

      if (!string.IsNullOrEmpty(_Application) && string.IsNullOrEmpty(auditTrail.Application))
      {
         auditTrail.Application = _Application;
      };
      
      // 
      try
      {
         AuditTrail.Add(auditTrail.Copy2AuditTrailList());
         OnPropertyChanged("AuditTrail");
      }
      catch { };

      // - - - Event - - - 

      if (CallBack != null)
      {
         try
         {
            bool Handled = false;
            CallBack(ref auditTrail, ref Handled);

            if (Handled) return true;
         }
         catch
         {
         };
      };

      // - - - Write to TXT - - - 
#if !NETCOREAPP1_0
      AddToFile(auditTrail);
#endif
      // - - - Write to DB - - - 

      if (_Connection != null)
      {
         try
         {
            if (string.IsNullOrEmpty(auditTrail.FKUser))
            {
               if (_User != null)
               {
                  auditTrail.FKUser = _User;
               }
            };

            if (!string.IsNullOrEmpty(_TerminalIP) && string.IsNullOrEmpty(auditTrail.TerminalIP))
            {
               auditTrail.TerminalIP = _TerminalIP;
            };

            if (!string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(auditTrail.ItemID))
            {
               auditTrail.ItemID = ItemID;
            };

            if (!string.IsNullOrEmpty(ItemType) && string.IsNullOrEmpty(auditTrail.ItemType))
            {
               auditTrail.ItemType = ItemType;
            };

#if WebService
#if !UT
            if (string.IsNullOrEmpty(auditTrail.TerminalID))
            {
               if (HttpContext.Current.Session != null)
               {
                  auditTrail.TerminalID = HttpContext.Current.Request.UserHostAddress;
               }
            };
#endif
#elif WINDOWS_UWP
            throw new NotImplementedException();
            // http://stackoverflow.com/questions/33736983/get-environment-variables-in-net-core-uwp
#else
            if (string.IsNullOrEmpty(auditTrail.TerminalID))
            {
               auditTrail.TerminalID = System.Environment.MachineName;
            };
#endif

            if (_Connection.State == System.Data.ConnectionState.Closed)
            {
               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };

               _Connection.Open();
            };

            StringToSQL(ref auditTrail);
            if (!DB_SQL.Insert(_Connection, "AuditTrail", auditTrail))
            {
               Debug.WriteLine(DB_SQL._ViewModel.LastError);
               Debug.WriteLine(DB_SQL._ViewModel.LastQuery);

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
            };
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(DB_SQL._ViewModel.LastError);
            Debug.WriteLine(DB_SQL._ViewModel.LastQuery);

            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
         };
      };

      // - - -  - - - 

      return true;
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
#if !NETCOREAPP1_0
#if PCL || NETFX_CORE 
   public async Task AddToFile(AuditTrail auditTrail)
   {
#endif
#if WPF || WebService || NETCOREAPP2_0 || NETSTANDARD2_0
   private void AddToFile(AuditTrail auditTrail)
   {
#endif
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   //public void AuditTrailCallBack(ErrorLevel Level, string Line)
   //{
   //   AuditTrail.Add(new AuditTrail() { TimeStamp = DateTime.Now, Level = Level, Message = Line });
   //}

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

#if PCL || NETFX_CORE 
   public async Task Clear()
   {
      AuditTrail.Clear();

      try
      {
         IFolder rootFolder = FileSystem.Current.LocalStorage;
         IFile file = await rootFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);

         if (file != null)
         {
            await file.DeleteAsync(System.Threading.CancellationToken.None);
         };
      }
      catch
      {
      };
   }
#endif

#if Desktop || WPF || NETSTANDARD2_0
   public bool ClearFile()
   {
      SelectedAuditTrail = null;
      AuditTrail.Clear();

      try
      {
         File.Delete(FileName);
      }
      catch { };

      // - - -  - - - 

      return true;
   }
#endif




#if Desktop || WPF  || NETSTANDARD2_0
   public bool Clear()
   {
      SelectedAuditTrail = null;
      AuditTrail.Clear();

      try
      {
         ClearFile();
      }
      catch { };

      // - - -  - - - 

      if (_Connection != null)
      {
         DB_SQL.QuickQuery(_Connection, "Drop table AuditTrail;");

         InitDB();
      };

      return true;
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

#if Desktop || WPF
   public enum CleanRange { none, TwoDays, Week, Month }

   public bool Clean(CleanRange cleanRange = CleanRange.none)
   {
      while (AuditTrail.Count > MaxLines) AuditTrail.RemoveAt(0);

      // - - -  - - - 

      DateTime dt = DateTime.MinValue;

      switch (cleanRange)
      {
         case CleanRange.none:
            break;

         case CleanRange.TwoDays:
            dt = DateTime.Now.AddDays(-2);
            break;

         case CleanRange.Week:
            dt = DateTime.Now.AddDays(-7);
            break;

         case CleanRange.Month:
            dt = DateTime.Now.AddMonths(-1);
            break;
      };

      // - - -  - - - 

      string FirstLine = "";

      if (_Connection != null && cleanRange!=CleanRange.none)
      {
         string SQL = string.Format("delete from AuditTrail where TimeStamp < {0}", DB_SQL.DateTimeToSQL(_Connection, dt));
         DB_SQL.QuickQuery(_Connection, SQL);

         // - - -  - - - 

         AuditTrail at = null;

         if (_Connection.GetType().ToString().Contains("SqlConnection"))
         {
            at = DB_SQL.QueryFirst<AuditTrail>(_Connection, "select top 1 * from AuditTrail");
         };

         if (_Connection.GetType().ToString().Contains("Npgsql"))
         {
            at = DB_SQL.QueryFirst<AuditTrail>(_Connection, "select * from AuditTrail limit 1");
         };

         if (_Connection.GetType().ToString().Contains("MySqlConnection"))
         {
            at = DB_SQL.QueryFirst<AuditTrail>(_Connection, "select * from AuditTrail limit 1");
         };

         //if (_Connection.GetType().ToString().Contains("sqlite"))
         //{
         // at = DB_SQL.QueryFirst<AuditTrail>(_Connection, "select * from AuditTrail limit 1");
         //};

         if (at != null)
         {
            FirstLine = FormatLine(at);
         }
      };

      // - - -  - - - 

      CleanFile(FirstLine);

      return true;
   }


   public bool CleanFile(string FirstLine)
   {
      bool res = false;
      try
      {
         StreamReader streamReader = null;

         try
         {
            streamReader = new StreamReader(FileName);
            System.Collections.ArrayList lines = new System.Collections.ArrayList();

            string line;
            bool DoIt = string.IsNullOrEmpty(FirstLine);

            while ((line = streamReader.ReadLine()) != null)
            {
               if (DoIt)
               {
                  lines.Add(line);
               }
               else
               {
                  if (line == FirstLine)
                  {
                     lines.Add(line);
                     DoIt = true;
                  }
               };
            }

            streamReader.Close();

            if ((lines.Count > MaxLines) || (!string.IsNullOrEmpty(FirstLine)))
            {
               StreamWriter streamWriter = new StreamWriter(FileName);

               int ind = (lines.Count - MaxLines < 0 ? 0 : lines.Count - MaxLines);

               for (int i = ind; i < lines.Count; i++)
               {
                  streamWriter.WriteLine(lines[i]);
               };

               streamWriter.Close();
            };
            res = true;
         }
         catch (OutOfMemoryException ex)
         {
            streamReader.Close();

            StreamWriter streamWriter = new StreamWriter(FileName);
            streamWriter.WriteLine("*** AuditTrail.Clean() ***");
            streamWriter.WriteLine(ex.ToString());
            streamWriter.Close();
         };
      }
      catch { };
      return res;
   }
#endif

   public Task Add(string message)
   {
      throw new NotImplementedException();

      //#if SQLLITE
      //      if (DB_SQL.SQLiteConnection != null)
      //      {
      //         DB_SQL.Insert(auditTrail);
      //      };
      //#endif
   }
#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   private void StringToSQL(ref AuditTrail at)
   {
#if PCL
      at.Application = DB_SQL.StringToSQL(at.Application);
      at.DataIn = DB_SQL.StringToSQL(at.DataIn);
      at.DataOut = DB_SQL.StringToSQL(at.DataOut);
      at.Message = DB_SQL.StringToSQL(at.Message);
      at.Tag = DB_SQL.StringToSQL(at.Tag);
#else
      at.Application = DB_SQL.StringToSQL(Connection, at.Application);
      at.DataIn = DB_SQL.StringToSQL(Connection, at.DataIn);
      at.DataOut = DB_SQL.StringToSQL(Connection, at.DataOut);
      at.Message = DB_SQL.StringToSQL(Connection, at.Message);
      at.Tag = DB_SQL.StringToSQL(Connection, at.Tag);
#endif
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

}

=======================================================================================================================================
=======================================================================================================================================


public static class Log
{
#if PCL
   public static async Task Write(ErrorLevel errorLevel, string Message,
                                  [CallerMemberName] string memberName = "",
                                  [CallerFilePath] string sourceFilePath = "",
                                  [CallerLineNumber] int sourceLineNumber = 0)
   {
      await Write(new AuditTrail()
      {
         Level = errorLevel,
         Message = Message,
      });
   }


   public static async Task Add(string Message)
   {
      await AuditTrailViewModel.Current.Add(Message);
   }
#else
   public static void Write(ErrorLevel errorLevel, string Message)
   {
      Write(new AuditTrail()
      {
         Tag = "",
         Level = errorLevel,
         Message = Message,
      });
   }
#endif


#if PCL
   public static async Task Write(string Tag, string Message)
   {
      await Write(new AuditTrail()
      {
         Tag = Tag,
         Message = Message,
      });
   }
#else
   public static void Write(string Tag, string Message)
   {
      Write(new AuditTrail()
      {
         Tag = Tag,
         Message = Message,
      });
   }

   public static void Write(string Tag, string Message, string Data)
   {
      Write(new AuditTrail()
      {
         Tag = Tag,
         Message = Message,
         DataOut = Data
      });
   }
#endif


#if PCL
   public static async Task Write(ErrorLevel errorLevel, Exception ex)
   {
      await Write(errorLevel, ex, "");
   }
#else
   public static void Write(ErrorLevel errorLevel, Exception ex)
   {
      Write(errorLevel, ex, "");
   }
#endif


#if PCL
   public static async Task Write(ErrorLevel errorLevel, Exception ex, string Data)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
      };

      at.DataOut = (string.IsNullOrEmpty(Data) ? at.DataOut : Data + Environment.NewLine + Environment.NewLine + at.DataOut);

      await Write(at);
   }
#else
   public static void Write(ErrorLevel errorLevel, Exception ex, string Data)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
      };

      at.DataOut = (string.IsNullOrEmpty(Data) ? at.DataOut : Data + Environment.NewLine + Environment.NewLine + at.DataOut);

      Write(at);
   }
#endif


#if PCL
   public static async Task Write(ErrorLevel errorLevel, string Tag, Exception ex)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
         Tag = Tag,
      };

      await Write(at);
   }
#else
   public static void Write(ErrorLevel errorLevel, string Tag, Exception ex)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
         Tag = Tag,
      };

      Write(at);
   }
#endif


#if PCL
   public static async Task Write(ErrorLevel errorLevel, string Tag, Exception ex, string Data)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
         Tag = Tag,
      };

      at.DataOut = (string.IsNullOrEmpty(Data) ? at.DataOut : Data + Environment.NewLine + Environment.NewLine + at.DataOut);

      await Write(at);
   }
#else
   public static void Write(ErrorLevel errorLevel, string Tag, Exception ex, string Data)
   {
      var at = new AuditTrail(ex)
      {
         Level = errorLevel,
         Tag = Tag,
      };

      at.DataOut = (string.IsNullOrEmpty(Data) ? at.DataOut : Data + Environment.NewLine + Environment.NewLine + at.DataOut);

      Write(at);
   }
#endif


#if PCL
   public static async Task Begin(AuditTrail auditTrail)
   {
      if (AuditTrailViewModel.Current.IsDBInit)
      {
         await AuditTrailViewModel.Current.Write(auditTrail);
      }
      else
      {
         await AuditTrailViewModel.Current.WriteTXT(auditTrail);
      }
   }

   public static async Task Write(AuditTrail auditTrail)
   {
      if (AuditTrailViewModel.Current.IsDBInit)
      {
         await AuditTrailViewModel.Current.Write(auditTrail);
      }
      else
      {
         await AuditTrailViewModel.Current.WriteTXT(auditTrail);
      }
   }
#else
   /// <summary>
   /// <code>
   /// var at = Log.Begin(new AuditTrail { IsBusiness = true, Tag = "Importation", Message = "Importation terminal", DataInType = "TXT", DataIn = FileList.Text });
   /// </code>
   /// </summary>
   /// <param name="auditTrail"></param>
   /// <returns></returns>
   public static Int64 Begin(AuditTrail auditTrail)
   {
      auditTrail.Ticks = DateTime.Now.Ticks;
      Int64 Result = AuditTrailViewModel.Current.Begin(auditTrail);
      auditTrail.Ticks = 0;

      return Result;
   }

   /// <summary>
   /// <code>
   /// var at = Log.Begin(new AuditTrail { IsBusiness = true, Tag = "Importation", Message = "Importation terminal", DataInType = "TXT", DataIn = FileList.Text });
   /// . . .
   /// Log.End(at, ErrorLevel.Log, "Importation terminée." );
   /// </code>
   /// </summary>
   /// <param name="auditTrail"></param>
   /// <returns></returns>
   public static void End(long Parent, ErrorLevel errorLevel, string Message, string DataOutType = "", string DataOut = "")
   {
      AuditTrailViewModel.Current.End(Parent, errorLevel, Message, DataOutType, DataOut);
   }

   public static void Write(AuditTrail auditTrail)
   {
      AuditTrailViewModel.Current.Write(auditTrail);
   }
#endif
   }


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public static void Debug(string Msg, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
   {
      System.Diagnostics.Debug.WriteLine(string.Format("[{0} {1:d4}] {2}", memberName, sourceLineNumber, Msg));
   }

#if PCL
   public static async Task WriteIfMsg(ErrorLevel errorLevel, string Message,
                                  [CallerMemberName] string memberName = "",
                                  [CallerFilePath] string sourceFilePath = "",
                                  [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!string.IsNullOrEmpty(Message))
      {
         await Write(errorLevel, Message, memberName, sourceFilePath, sourceLineNumber);
      };
   }

#else
   public static void WriteIfMsg(ErrorLevel errorLevel, string Message,
                                  [CallerMemberName] string memberName = "",
                                  [CallerFilePath] string sourceFilePath = "",
                                  [CallerLineNumber] int sourceLineNumber = 0)
   {
      if (!string.IsNullOrEmpty(Message))
      {
         Write(new AuditTrail()
         {
            Level = errorLevel,
            Message = Message,
            DataOut = string.Format("[{0} {1:d4}]", memberName, sourceLineNumber),
         });
      };
   }

#endif

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
#endif
}
*/
