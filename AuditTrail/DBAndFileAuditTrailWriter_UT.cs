using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF.AT
{
   [TestClass]
   public class DBAndFileAuditTrailWriter_UT
   {
      public static string AuditTrailFileName = "AT_UT.log";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void Init_AT_PG()
      {
         string server = "postgresql-zpf.alwaysdata.net";
         string db = "zpf_postgresql";
         string user = "zpf_wanao";
         string password = "wanao_zpf";

         DBSQLViewModel AuditTrailConnection = null;
         AuditTrailConnection = new DBSQLViewModel(new PostgreSQLEngine());
         string AT_ConnectionString = DB_SQL.GenConnectionString(DBType.PostgreSQL, server, db, user, password);
         AuditTrailConnection.Open(AT_ConnectionString, true);

         AuditTrailViewModel.Current.Init(new DBAndFileAuditTrailWriter(AuditTrailConnection, string.Format(@"{0}", AuditTrailFileName)));
         AuditTrailViewModel.Current.MaxLines = 5;
         AuditTrailViewModel.Current.Clean(); // prend en compte uniquement nombre ligne

         Assert.AreEqual(true, AuditTrailConnection.CheckConnection());
      }

      [TestMethod]
      public void Init_AT_SQLServer()
      {
         string server = "SQL6005.site4now.net";
         string db = "DB_A44F11_Test";
         string user = "DB_A44F11_Test_admin";
         string password = "MossIsTheBoss19";

         DBSQLViewModel AuditTrailConnection = null;
         AuditTrailConnection = new DBSQLViewModel(new SQLServerEngine());
         string AT_ConnectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);
         AuditTrailConnection.Open(AT_ConnectionString, true);

         AuditTrailViewModel.Current.Init(new DBAndFileAuditTrailWriter(AuditTrailConnection, string.Format(@"{0}", AuditTrailFileName)));
        // AuditTrailViewModel.Current.MaxLines = 5;
         //AuditTrailViewModel.Current.Clean(); // prend en compte uniquement nombre ligne

         Assert.AreEqual(true, AuditTrailConnection.CheckConnection());
      }

      [TestMethod]
      public void Init_AT_SQLServer2()
      {
         string server = "SQL6005.site4now.net";
         string db = "DB_A44F11_StockAPPro2dev";
         string user = "DB_A44F11_StockAPPro2dev_admin";
         string password = "MossIsTheBoss19";

         DBSQLViewModel AuditTrailConnection = null;
         AuditTrailConnection = new DBSQLViewModel(new SQLServerEngine());
         string AT_ConnectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);
         AuditTrailConnection.Open(AT_ConnectionString, true);

         AuditTrailViewModel.Current.Init(new DBAndFileAuditTrailWriter(AuditTrailConnection, string.Format(@"{0}", AuditTrailFileName)));
         // AuditTrailViewModel.Current.MaxLines = 5;
         //AuditTrailViewModel.Current.Clean(); // prend en compte uniquement nombre ligne

         Assert.AreEqual(true, AuditTrailConnection.CheckConnection());
      }

      [TestMethod]
      public void Init_AT_File()
      {
         File.Copy(AuditTrailFileName, @"_" + DateTime.Now.ToString("yy_MM_dd_HH_mm_ss_fff") + AuditTrailFileName);
         File.Delete(AuditTrailFileName);
         string rep = Directory.GetCurrentDirectory();
         AuditTrailViewModel.Current.Init(new FileAuditTrailWriter(string.Format(@"{0}", AuditTrailFileName)));
         AuditTrailViewModel.Current.MaxLines = 5;
         AuditTrailViewModel.Current.Clean(); // prend en compte uniquement nombre ligne

         Assert.AreEqual(true, File.Exists(AuditTrailFileName));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void Log_Begin_AT_PG()
      {
         Init_AT_PG();

         long res = Log.Begin(new AuditTrail { Level = ErrorLevel.Info, Tag = "Tag", Message = "test" });
         Log.End(res,ErrorLevel.Info,$"Message Fin pour AT {res}");
         Assert.AreNotEqual(-1, res);
      }

      [TestMethod]
      public void Log_Begin_AT_SQLServer()
      {
         Init_AT_SQLServer();

         long res = Log.Begin(new AuditTrail { Level= ErrorLevel.Info, Tag= "Tag", Message = "test" });
         Log.End(res, ErrorLevel.Info, $"Message Fin pour AT {res}");
         Assert.AreNotEqual(-1, res);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
