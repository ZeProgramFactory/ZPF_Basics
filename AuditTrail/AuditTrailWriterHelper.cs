using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZPF.SQL;

namespace ZPF.AT
{
   public class AuditTrailWriterHelper
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static string GetWhere(DBType dbType, AuditTrailViewModel sender, bool ShowUser = true)
      {
         // String SQL = "SELECT TOP 500 AuditTrail.*, UserAccount.Login FROM AuditTrail LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = UserAccount.PK WHERE 1=1 ORDER BY AuditTrail.PK DESC";
         String Where = "";

         switch (dbType)
         {
            case DBType.MySQL:
               // Where = "LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = cast(UserAccount.PK as varchar(128)) WHERE 1=1 order by AuditTrail.PK desc";
               Where = "WHERE 1=1 order by AuditTrail.PK desc";
               break;

            default:
               // Where = "LEFT OUTER JOIN UserAccount ON AuditTrail.FKUser = cast(UserAccount.PK as varchar(128)) WHERE 1=1 order by AuditTrail.PK desc";
               Where = "WHERE 1=1 order by AuditTrail.PK desc";
               break;
         };


         if (!ShowUser)
         {
            Where = "WHERE 1=1 order by AuditTrail.PK desc";
         };

         // - -  - -

         Where = Where.Replace("1=1", string.Format("Level>={0} and 1=1", (int)(sender.Level)));

         if (!sender.Childs)
         {
            Where = Where.Replace("1=1", "(Parent is null or Parent=0) and 1=1");
         };

         if (!sender.IsDebug)
         {
            Where = Where.Replace("1=1", "IsBusiness = '1' and 1=1");
         };

         if (!string.IsNullOrEmpty(sender.TerminalID))
         {
            Where = Where.Replace("1=1", string.Format("TerminalID = '{0}' and 1=1", sender.TerminalID));
         };

         //if (!string.IsNullOrEmpty(sender.User) && sender.User != "0")
         //{
         //   Where = Where.Replace("1=1", string.Format("FKUser = '{0}' and 1=1", User));
         //};

         //if (!string.IsNullOrEmpty(sender.UserWhere) && sender.User != "0")
         //{
         //   UserWhere = UserWhere.Trim();

         //   if (UserWhere.StartsWith("AND "))
         //   {
         //      Where = Where.Replace("1=1", $"{UserWhere.Substring(5)} and 1=1");
         //   }
         //   else if (UserWhere.StartsWith("OR "))
         //   {
         //      Where = Where.Replace("1=1", $"{UserWhere.Substring(4)} or 1=1");
         //   }
         //   else if (UserWhere.StartsWith("("))
         //   {
         //      if (!UserWhere.EndsWith(")"))
         //      {
         //         UserWhere = UserWhere + ")";
         //      };

         //      Where = Where.Replace("1=1", $"{UserWhere} and 1=1");
         //   }
         //   else
         //   {
         //      Where = Where.Replace("1=1", $"{UserWhere} and 1=1");
         //   };
         //};

         //if (!string.IsNullOrEmpty(Event) && Event != "01/01/1900 00:00:00")
         //{
         //   Where = Where.Replace("1=1", string.Format(" and ItemType like '{0}%' ", Event.Left(16)));
         //};

         return Where;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static ObservableCollection<AuditTrail> LoadAuditTrail(DBSQLViewModel dBSQLViewModel, AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
      {
         var AuditTrail = new List<AuditTrail>();

         string Where = GetWhere(dBSQLViewModel.DBType, sender);

         string SQL = DB_SQL.SelectAll(dBSQLViewModel.DBType, "AuditTrail", Where, MaxRecords);

         if (dBSQLViewModel != null)
         {
            try
            {
               AuditTrail = DB_SQL.Query<AuditTrail>(dBSQLViewModel, SQL);

               if (AuditTrail == null)
               {
                  AuditTrail = new List<AuditTrail>();
               };
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.Message);
            };
         };

         return new ObservableCollection<AuditTrail>(AuditTrail);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static void WriteLineDB(DBSQLViewModel dBSQLViewModel, AuditTrailViewModel sender, AuditTrail message)
      {
         if (dBSQLViewModel == null) return;

         try
         {
            DB_SQL.Insert(dBSQLViewModel, message);

            if (!string.IsNullOrEmpty(dBSQLViewModel.LastError))
            {
               Debug.WriteLine("AuditTrailViewModel LastError: " + dBSQLViewModel.LastError + Environment.NewLine
                              + "AuditTrailViewModel LastQuery: " + dBSQLViewModel.LastQuery);

               //Log.Debug(new AuditTrail
               //{
               //   Message = "AuditTrailViewWriter LastError: " + dBSQLViewModel.LastError + Environment.NewLine
               //                                       + "AuditTrailViewWriter LastQuery: " + dBSQLViewModel.LastQuery
               //});
            };
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine("AuditTrailViewModel: " + ex.Message);

            //Log.Debug(new AuditTrail
            //{
            //   Message = "AuditTrailViewWriter: " + ex.Message
            //});
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static long BeginWriteDB(DBSQLViewModel dBSQLViewModel, AuditTrailViewModel sender, AuditTrail message)
      {
         long Result = -1;

         try
         {
            Result = DB_SQL.InsertGetPK(dBSQLViewModel, message);
            if (!string.IsNullOrEmpty(dBSQLViewModel.LastError))
            {
               Debug.WriteLine("AuditTrailViewWriter LastError: " + dBSQLViewModel.LastError + Environment.NewLine
                              + "AuditTrailViewWriter LastQuery: " + dBSQLViewModel.LastQuery);

               Log.Write(new AuditTrail
               {
                  Message = "AuditTrailViewWriter LastError: " + dBSQLViewModel.LastError + Environment.NewLine
                                                      + "AuditTrailViewWriter LastQuery: " + dBSQLViewModel.LastQuery
               });
            }
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine("AuditTrailViewWriter: " + ex.Message);

            Log.Write(new AuditTrail
            {
               Message = "AuditTrailViewWriter: " + ex.Message
            });
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static bool EndWriteDB(DBSQLViewModel dBSQLViewModel, Int64 parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
      {
         long Ticks = DateTime.Now.Ticks;

         var at = DB_SQL.QueryFirst<AuditTrail>(dBSQLViewModel, "select * from AuditTrail where PK=" + parent);

         if (at != null)
         {
            at.Level = errorLevel;
            at.Ticks = Ticks - at.Ticks;
            at.DataOutType = dataOutType;
            at.DataOut = dataOut;
            at.Parent = -1; // ChM 20180514 pour simplifier requetes exploitations begin sans end

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

            DB_SQL.Insert(dBSQLViewModel, endAT);

            return DB_SQL.Update(dBSQLViewModel, at);
         }
         else
         {
            return false;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public static bool CreateTable(DBSQLViewModel _dBSQLViewModel = null)
      {
         bool Result = true;
         if (!_dBSQLViewModel.CheckConnection()) return false;

         DB_SQL.QuickQueryInt(_dBSQLViewModel, "select PK from AuditTrail where 1=2");
         if (DB_SQL._ViewModel != null && DB_SQL._ViewModel.LastError == "")
         {
            return true;
         };

         if (Result)
         {
            // - - -  - - - 

            #region Create table & co 

            string SQL = "";

            switch (_dBSQLViewModel.DBType)
            {
               case DBType.Firebird: return false;    // ToDo CHM 

               case DBType.SQLServer: SQL = AuditTrail.PostScript_MSSQL; break;

               case DBType.SQLite: SQL = AuditTrail.PostScript_SQLite; break;

               case DBType.PostgreSQL: SQL = AuditTrail.PostScript_PGSQL; break;

               case DBType.MySQL: SQL = AuditTrail.PostScript_MySQL; break;
            };

            // - - -  - - - 

            DB_SQL.CreateTable(_dBSQLViewModel, typeof(AuditTrail), SQL, "");

            #endregion

            // - - -  - - - 

            if (!Result)
            {
               Log.Write(new AuditTrail() { Level = ErrorLevel.Error, Message = _dBSQLViewModel.LastError + Environment.NewLine + _dBSQLViewModel.LastQuery });

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };

               return false;
            }
         };
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   }
}
