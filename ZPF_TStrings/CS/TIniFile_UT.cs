using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZPF;

namespace ZPF
{
   // http://www.scribdbook.com/unit-testing-frameworks-in-c-comparing-xunit-nunit-and-visual-studio/
   // https://blogs.msdn.microsoft.com/visualstudio/2016/11/18/live-unit-testing-visual-studio-2017-rc/
   // https://ict.ken.be/unit-testing-with-mstest

   [TestClass]
   public class TIniFile_UT
   {
      string FileName = "";

      [TestMethod]
      public void TIniFile_Save_Test_1()
      {
         FileName = System.IO.Path.GetTempFileName() + ".ini";

         TIniFile ini = new TIniFile(FileName);
         ini.WriteString("Section", "Now", DateTime.Now.ToString());
         ini.WriteString("Section", "Test", "123");
         ini.UpdateFile();

         bool Exist = System.IO.File.Exists(FileName);

         Assert.AreEqual(true, Exist);
      }

      [TestMethod]
      public void TIniFile_Load_Test_1()
      {
         TIniFile_Save_Test_1();

         TIniFile ini = new TIniFile(FileName);
         string Result = ini.ReadString("Section", "Test", "oups");

         Assert.AreEqual("123", Result);
      }

   }
}
