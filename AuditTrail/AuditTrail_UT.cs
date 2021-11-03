using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
      public void AuditTrail_JSON01()
      {
         var path = System.IO.Path.GetTempFileName();

         AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(path));

         Log.WriteHeader("Cassini", "V1.23", $"");

         AuditTrailViewModel.Current.Logs.Clear();

         var DT = DateTime.Now;


         var ts = DateTime.Now - DT;
         Log.Write(ErrorLevel.Info, ts.TotalSeconds.ToString());

         Assert.AreEqual(true, AuditTrailViewModel.Current.Logs.Count == 4);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_JSON02()
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

      [TestMethod]
      public void AuditTrail_JSON2CSV()
      {
         var path = System.IO.Path.GetTempFileName();

         AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(path));
         AuditTrailViewModel.Current.Logs.Clear();

         var DT = DateTime.Now;

         for (int i = 0; i < 10; i++)
         {
            Log.Write(new AuditTrail { Message = "mess0" });
            Log.Write(ErrorLevel.Info, "mess1");
            Log.Write("tag", "mess2");
            Log.Write(ErrorLevel.Info, new Exception("mess3"));
         };

         var ts = DateTime.Now - DT;
         Log.Write(ErrorLevel.Info, ts.TotalSeconds.ToString());

         var fn = path + ".csv";

         AuditTrailViewModel.Current.LoadAuditTrail();

         ListToCSV(AuditTrailViewModel.Current.AuditTrail.ToList(), fn);

         Assert.AreEqual(true, AuditTrailViewModel.Current.Logs.Count == 4);
      }

      /// <summary>
      /// Creates the CSV from a generic list.
      /// </summary>;
      /// <typeparam name="T"></typeparam>;
      /// <param name="list">The list.</param>;
      /// <param name="csvFileName">Name of CSV (w/ path) w/ file ext.</param>;
      void ListToCSV<T>(List<T> list, string csvFileName)
      {
         if (list == null || list.Count == 0)
         {
            return;
         };

         if (!Directory.Exists(Path.GetDirectoryName(csvFileName)))
         {
            Directory.CreateDirectory(Path.GetDirectoryName(csvFileName));
         };

         using (var sw = new StreamWriter(csvFileName))
         {
            // - - - gets all properties - - - 

            PropertyInfo[] properties = typeof(T).GetProperties();

            // - - - create header - - -

            for (int i = 0; i < properties.Length - 1; i++)
            {
               var prop = properties[i];

               if (Attribute.GetCustomAttribute(prop, typeof(DB_Attributes.IgnoreAttribute)) == null && Attribute.GetCustomAttribute(prop, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) == null)
               {
                  sw.Write(prop.Name + ",");
               };
            };

            sw.Write(sw.NewLine);

            // - - - create rows - - -

            foreach (var item in list)
            {
               for (int i = 0; i < properties.Length - 1; i++)
               {
                  var prop = properties[i];

                  if (Attribute.GetCustomAttribute(prop, typeof(DB_Attributes.IgnoreAttribute)) == null && Attribute.GetCustomAttribute(prop, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) == null)
                  {
                     if (prop.PropertyType.Name == "String")
                     {
                        sw.Write($@"""{prop.GetValue(item)}"",");
                     }
                     else
                     {
                        sw.Write(prop.GetValue(item) + ",");
                     };
                  };
               };

               sw.Write(sw.NewLine);
            }
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_FromHere()
      {
         var at = AuditTrail.FromHere(ErrorLevel.Log, "tag", "message");

         Assert.AreEqual(true, at.DataIn.Contains("AuditTrail_FromHere"));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      [TestMethod]
      public void AuditTrail_WithStack()
      {
         var at = AuditTrail.WithStack(ErrorLevel.Log, "tag", "message");

         Assert.AreEqual(true, at.DataIn.Contains("AuditTrail_WithStack"));
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
      [TestMethod]
      public void AuditTrail_JSONClean()
      {
         var path = System.IO.Path.GetTempFileName();

         AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter("AT_TestClean.txt"));
         Log.WriteHeader("AT TestClean", "V0.1", $"");
         //for (int i = 0; i < 2500; i++)
         //{
         //   Log.Write(ErrorLevel.Info, $"Test {i}");
         //}
         ZPF.AT.AuditTrailViewModel.Current.MaxLines = 200;
         ZPF.AT.AuditTrailViewModel.Current.Clean();

         Log.WriteHeader("AT TestClean", "V0.11", $"");

         // AuditTrailViewModel.Current.Logs.Clear();

         var DT = DateTime.Now;


         var ts = DateTime.Now - DT;
         Log.Write(ErrorLevel.Info, ts.TotalSeconds.ToString());

         Assert.AreEqual(true, AuditTrailViewModel.Current.Logs.Count == 4);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   }
}
