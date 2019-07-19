using System;

namespace ZPF.SQL
{
   /// <summary>
   /// 11/05/17 - ME  - Add: IgnoreIntTypeAttribute
   /// 17/05/17 - ME  - Add: IgnoreBoolTypeAttribute
   /// 
   /// 2005..2019 ZePocketForge.com, SAS ZPF
   /// </summary>
   public class DB_Attributes
   {
      [AttributeUsage(AttributeTargets.Property)]
      public class PrimaryKeyAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreIntTypeAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreBoolTypeAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class MaxLengthAttribute : Attribute
      {
         public MaxLengthAttribute(int Length)
         {
         }
      }
   }
}
