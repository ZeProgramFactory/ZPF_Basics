using System;

namespace ZPF.SQL
{
   /// <summary>
   /// 11/05/17 - ME  - Add: IgnoreIntTypeAttribute
   /// 17/05/17 - ME  - Add: IgnoreBoolTypeAttribute
   /// 02/06/20 - ME  - Add: presentation attributes
   /// 28/03/21 - ME  - Add: Table creation attributes
   /// 
   /// 2005..2021 ZePocketForge.com, SAS ZPF, @ZeProgFractory
   /// </summary>
   public enum Alignments { left, center, right }
   public enum SortDirections { asc, desc }


   public class DB_Attributes
   {
      [AttributeUsage(AttributeTargets.Property)]
      public class PrimaryKeyAttribute : Attribute
      {
         public PrimaryKeyAttribute()
         {
            SortDirection = SortDirections.asc;
         }

         public PrimaryKeyAttribute( bool autoInc = false, SortDirections sortDirection = SortDirections.asc)
         {
            AutoInc = autoInc;
            SortDirection = sortDirection;
         }

         public bool AutoInc { get; }
         public SortDirections SortDirection { get; }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IndexAttribute : Attribute
      {
         public IndexAttribute()
         {
            SortDirection = SortDirections.asc;
         }

         public IndexAttribute(bool isUnique = false, SortDirections sortDirection = SortDirections.asc, string indexedColumns="" )
         {
            IsUnique = isUnique;
            SortDirection = sortDirection;
            IndexedColumns = indexedColumns;
         }

         public bool IsUnique { get; }
         public SortDirections SortDirection { get; }
         public string IndexedColumns { get; }
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
         public TableNameAttribute(string tableName)
         {
            TableName = tableName;
         }

         public string TableName { get; }
      }

      [AttributeUsage(AttributeTargets.Class)]
      public class TableCommentAttribute : Attribute
      {
         public TableCommentAttribute(string comment)
         {
            Comment = comment;
         }

         public string Comment { get; }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class CommentAttribute : Attribute
      {
         public CommentAttribute(string comment)
         {
            Comment = comment;
         }

         public string Comment { get; }
      }

      // - - -  - - - 

      /// <summary>
      /// Creates a virtual field that will be created in the DB tables 
      /// but not stored in memory (e.g. prevent that blobd are loaded into memory)
      /// </summary>
      [AttributeUsage(AttributeTargets.Property)]
      public class VirtualFieldAttribute : Attribute
      {
         public VirtualFieldAttribute(IField field)
         {
            Field = field;
         }

         public IField Field { get; }
      }

      // - - -  - - - 

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreImportAttribute : Attribute
      {
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class ImportFieldIndexAttribute : Attribute
      {
         public ImportFieldIndexAttribute(int index)
         {
            Index = index;
         }

         public int Index { get; }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class ImportFieldNameAttribute : Attribute
      {
         public ImportFieldNameAttribute(string colName)
         {
            ColName = colName;
         }

         public string ColName { get; }
      }

      [AttributeUsage(AttributeTargets.Property)]
      public class IgnoreExportAttribute : Attribute
      {
      }

      // - - -  - - - 

      [AttributeUsage(AttributeTargets.Property)]
      public class MaxLengthAttribute : Attribute
      {
         public MaxLengthAttribute(int length)
         {
            Length = length;
         }
         public int Length { get; }
      }

      /// <summary>
      /// For example, the constant 12.345 is converted into a numeric value with a precision of 5 and a scale of 3. 
      /// </summary>
      [AttributeUsage(AttributeTargets.Property)]
      public class PrecisionScaleAttribute : Attribute
      {
         public PrecisionScaleAttribute()
         {
            Precision = 10;
            Scale = 2;
         }

         public PrecisionScaleAttribute(int precision)
         {
            Precision = precision;
            Scale = 2;
         }

         public PrecisionScaleAttribute(int precision, int scale)
         {
            Precision = precision;
            Scale = scale;
         }

         public int Precision { get; }
         public int Scale { get; }
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
