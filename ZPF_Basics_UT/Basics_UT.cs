using System;
using System.Collections.Generic;
using System.Text;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZPF;

namespace ZPF
{
   [TestClass]
   public class Basics_UT
   {
      string input2 = "one" + Environment.NewLine + "two" + Environment.NewLine;
      string input3 = "one" + Environment.NewLine + "two" + Environment.NewLine + "three" + Environment.NewLine;
      string input4 = "one" + Environment.NewLine + "two" + Environment.NewLine + "three" + Environment.NewLine + "four" + Environment.NewLine;

      [TestMethod]
      public void StringExt_Top020()
      {
         var output = input2.Top(3);

         var lines = new TStrings();
         lines.Text = output;

         Assert.AreEqual(2, lines.Count);
      }

      [TestMethod]
      public void StringExt_Top030()
      {
         var output = input3.Top(3);

         var lines = new TStrings();
         lines.Text = output;

         Assert.AreEqual(3, lines.Count);
      }

      [TestMethod]
      public void StringExt_Top040()
      {
         var output = input4.Top(3);

         var lines = new TStrings();
         lines.Text = output;

         Assert.AreEqual(3, lines.Count);
      }
   }
}
