using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZPF
{
   [TestClass]
   public class StringExtensions_UT
   {
      [TestMethod]
      public void _01_InDoubleQuote_00()
      {
         var st = @"";

         bool result = st.InDoubleQuotes();

         Assert.AreEqual(false, result);
      }

      [TestMethod]
      public void _01_InDoubleQuote_01()
      {
         var st = @"

""123"", ""bla bla \r\n bla bla"", ""Alpha 123"", ""signature \r\n Tom Jones

";

         bool result = st.InDoubleQuotes();

         Assert.AreEqual(true, result);
      }

      [TestMethod]
      public void _01_InDoubleQuote_02()
      {
         var st = @"

""123"", ""bla bla \r\n bla bla"", ""Alpha 123"", ""signature \r\n Tom Jones"", ..., \r\n

";

         bool result = st.InDoubleQuotes();

         Assert.AreEqual(false, result);
      }

      [TestMethod]
      public void _02_GetLines_01()
      {
         var st = @"
""T1"",""T2"",""Value""
   ""123"", ""bla bla 
bla bla"", 123 
""signature \r\n Tom Jones"", ""ABC"", 456

";
         var lines = st.GetLines();

         Assert.AreEqual(3, lines.Count);
      }

      [TestMethod]
      public void _02_GetLines_02()
      {
         var text = System.IO.File.ReadAllLines(@"D:\GitWare\Nugets\ZPF_MCE\BP2\Doc\Voies Paris.csv");

         var lines = text.GetLines();

         Assert.AreEqual(true, lines.Count == 6652);
      }

      [TestMethod]
      public void _02_GetLines_03()
      {
         var text = System.IO.File.ReadAllLines(@"D:\GitWare\Nugets\ZPF_Basics\Data\AuditTrail.csv");

         var lines = text.GetLines();

         Assert.AreEqual(true, lines.Count == 2001);
      }


      [TestMethod]
      public void _02_GetLines_04()
      {
         var text = System.IO.File.ReadAllText(@"D:\GitWare\Nugets\ZPF_Basics\Data\AuditTrail.csv").Replace( Environment.NewLine, "\n");

         var lines = text.GetLines("\n");

         Assert.AreEqual(true, lines.Count == 2001);
      }
   }
}
