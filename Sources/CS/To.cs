using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

public class To
{
   private static NumberFormatInfo GetNumberFormatInfo()
   {
      NumberFormatInfo Result = new NumberFormatInfo();

      Result.NumberDecimalSeparator = ".";
      Result.NumberGroupSeparator = " ";
      Result.NumberGroupSizes = new int[] { 3 };

      return Result;
   }

   public static Double Double(string Value)
   {
      Value = Value.Replace(",", ".");
      return Convert.ToDouble(Value, GetNumberFormatInfo());
   }

   public static Decimal Decimal(string Value)
   {
      Value = Value.Replace(",", ".");
      return Convert.ToDecimal(Value, GetNumberFormatInfo());
   }

   public static string String(decimal Value)
   {
      return To.String("{0}", Value);
   }

   public static string String(string Format, double Value)
   {
      return Value.ToString(Format, GetNumberFormatInfo());
   }

   public static string String(string Format, Decimal Value)
   {
      return Value.ToString(Format, GetNumberFormatInfo());
   }
}
