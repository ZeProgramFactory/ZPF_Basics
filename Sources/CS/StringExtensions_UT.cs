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
   }
}
