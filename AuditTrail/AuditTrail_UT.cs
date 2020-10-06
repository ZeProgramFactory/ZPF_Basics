using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZPF.AT;
using ZPF.SQL;

namespace ZPF.AT
{
   [TestClass]
   public class AuditTrail_UT
   {
      public static string AuditTrailFileName = "AT_UT.log";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_SQLCreate_MSSQL()
      {
         //string server = "SQL6005.site4now.net";
         //string db = "DB_A44F11_Test";
         //string user = "DB_A44F11_Test_admin";
         //string password = "MossIsTheBoss19";

         //DBSQLViewModel AuditTrailConnection = null;
         //AuditTrailConnection = new DBSQLViewModel(new SQLServerEngine());
         //string AT_ConnectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);
         //AuditTrailConnection.Open(AT_ConnectionString, true);

         //DB_SQL.QuickQuery("Drop view V_AuditTrail_Last100;");
         //DB_SQL.QuickQuery("Drop table AuditTrail;");

         //string result = DB_SQL.QuickQuery(AuditTrail.SQLCreate_MSSQL);

         //Assert.AreEqual("-1", result);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_Memory()
      {
         AuditTrailViewModel.Current.Init(null);
         AuditTrailViewModel.Current.Logs.Clear();


         Log.Write(new AuditTrail { Message = "mess0" });
         Log.Write(ErrorLevel.Info, "mess1");
         Log.Write("tag", "mess2");
         Log.Write(ErrorLevel.Info, new Exception("mess3"));


         Assert.AreEqual(true, AuditTrailViewModel.Current.Logs.Count == 4);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_JSON()
      {
         var path = System.IO.Path.GetTempFileName();

         AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(path));
         AuditTrailViewModel.Current.Logs.Clear();

         var DT = DateTime.Now;

         for (int i = 0; i < 300; i++)
         {
            Log.Write(new AuditTrail { Message = "mess0" });
            Log.Write(ErrorLevel.Info, "mess1");
            Log.Write("tag", "mess2");
            Log.Write(ErrorLevel.Info, new Exception("mess3"));
         };

         var ts = DateTime.Now - DT;
         Log.Write(ErrorLevel.Info, ts.TotalSeconds.ToString());


         Assert.AreEqual(true, AuditTrailViewModel.Current.Logs.Count == 4);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
