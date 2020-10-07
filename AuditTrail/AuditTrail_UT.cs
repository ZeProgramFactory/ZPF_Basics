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
      /// <param name="csvNameWithExt">Name of CSV (w/ path) w/ file ext.</param>;
      void ListToCSV<T>(List<T> list, string csvCompletePath)
      {
         if (list == null || list.Count == 0) return;

         // get type from 0th member
         Type t = list[0].GetType();
         string newLine = Environment.NewLine;

         if (!Directory.Exists(Path.GetDirectoryName(csvCompletePath))) Directory.CreateDirectory(Path.GetDirectoryName(csvCompletePath));

         //if (!File.Exists(csvCompletePath)) File.Create(csvCompletePath);

         using (var sw = new StreamWriter(csvCompletePath))
         {
            // make a new instance of the class name we figured out to get its props
            object o = Activator.CreateInstance(t);

            // gets all properties
            PropertyInfo[] props = o.GetType().GetProperties();

            // foreach of the properties in class above, write out properties
            // this is the header row
            sw.Write(string.Join(",", props.Select(d => d.Name).ToArray()) + newLine);

            CreateRows(list, sw);
            //foreach (T item in list)
            //{
            //   // this acts as datacolumn

            //   var row = string.Join(",", props.Select(d => item.GetType().GetProperty(d.Name).GetValue(item, null).ToString()).ToArray());

            //   sw.Write(row + newLine);
            //}
         }
      }

      private static void CreateRows<T>(List<T> list, StreamWriter sw)
      {
         foreach (var item in list)
         {
            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < properties.Length - 1; i++)
            {
               var prop = properties[i];

               if (prop.PropertyType.Name == "String")
               {
                  sw.Write($@"""{prop.GetValue(item)}"",");
               }
               else
               {
                  sw.Write(prop.GetValue(item) + ",");
               };
            };

            var lastProp = properties[properties.Length - 1];

            sw.Write(lastProp.GetValue(item) + sw.NewLine);
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
