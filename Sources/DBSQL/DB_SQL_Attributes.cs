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
   public enum Alignments { left, center, right }

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

      [AttributeUsage(AttributeTargets.Class)]
      public class ImportFieldIndexAttribute : Attribute
      {
         public ImportFieldIndexAttribute(int Index)
         {
         }
      }

      [AttributeUsage(AttributeTargets.Class)]
      public class ImportFieldNameAttribute : Attribute
      {
         public ImportFieldNameAttribute(string ColName)
         {
         }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreImportAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreExportAttribute : Attribute
      {
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
      public class DisplayOrderAttribute : Attribute
      {
         public DisplayOrderAttribute(int displayOrder)
         {
            DisplayOrder = displayOrder;
         }

         public int DisplayOrder { get; }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class ShowAttribute : Attribute
      {
         public ShowAttribute(bool show)
         {
            Show = show;
         }

         public ShowAttribute(bool show, int width = -1, Alignments alignment = Alignments.left, string header = null, string formatStr = null, bool hideIfNull = false, bool isBold = false)
         {
            Show = show;
            Width = width;
            Alignment = alignment;
            Header = header;
            FormatStr = formatStr;
            HideIfNull = hideIfNull;
            IsBold = isBold;
         }

         public bool Show { get; }
         public int Width { get; }
         public Alignments Alignment { get; }
         public string Header { get; }
         public string FormatStr { get; }
         public bool HideIfNull { get; }
         public bool IsBold { get; }
      }

      // - - -  - - - 
   }
}
