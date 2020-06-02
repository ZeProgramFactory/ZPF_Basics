using System;

namespace ZPF.SQL
{
   /// <summary>
   /// 11/05/17 - ME  - Add: IgnoreIntTypeAttribute
   /// 17/05/17 - ME  - Add: IgnoreBoolTypeAttribute
   /// 02/06/20 - ME  - Add: presentation attributes
   /// 
   /// 2005..2020 ZePocketForge.com, SAS ZPF
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

      [AttributeUsage(AttributeTargets.Class)]
      public class TableNameAttribute : Attribute
      {
         public TableNameAttribute(string TableName)
         {
         }
      }

      [AttributeUsage(AttributeTargets.Class)]
      public class CommentAttribute : Attribute
      {
         public CommentAttribute(string Comment)
         {
         }
      }

      // - - -  - - - 

      [AttributeUsage(AttributeTargets.Property)]
      public class MaxLengthAttribute : Attribute
      {
         public MaxLengthAttribute(int Length)
         {
         }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IsMandatoryAttribute : Attribute
      {
      }

      // - - - presentation - - - 

      [AttributeUsage(AttributeTargets.Property)]
      public class HideKeyAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class WidthAttribute : Attribute
      {
         public WidthAttribute(decimal Width)
         {
         }
      }

      public enum Alignments { left, center, right }

      [AttributeUsage(AttributeTargets.Property)]
      public class AlignmentAttribute : Attribute
      {
         public AlignmentAttribute(Alignments alignment)
         {
         }
      }

      // - - -  - - - 
   }
}
