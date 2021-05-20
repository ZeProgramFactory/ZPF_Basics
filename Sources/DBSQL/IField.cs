using System;
using System.Collections.Generic;
using System.Text;

namespace ZPF.SQL
{
   public interface IField
   {
      string FieldName { get; set; }

      [DB_Attributes.IgnoreBoolType]
      bool IsPrimaryKey { get; set; }

      [DB_Attributes.IgnoreBoolType]
      bool IsAutoInc { get; set; }

      [DB_Attributes.IgnoreBoolType]
      [DB_Attributes.Comment("could be null --> not mandatory")]
      bool IsMandatory { get; set; }

      string FieldType { get; set; }

      [DB_Attributes.IgnoreIntType]
      Int64 MaxSize { get; set; }

      string DefaultValue { get; set; }

      string Description { get; set; }

      // - - - Tiny SQL internal use - - -
      bool IsVisible { get; set; }
      int DisplayWidth { get; set; }
   }

   public class BasicField : IField
   {
      string IField.FieldName { get ; set ; }
      bool IField.IsPrimaryKey { get ; set ; }
      bool IField.IsAutoInc { get ; set ; }
      bool IField.IsMandatory { get ; set ; }
      string IField.FieldType { get ; set ; }
      long IField.MaxSize { get ; set ; }
      string IField.DefaultValue { get ; set ; }
      string IField.Description { get ; set ; }
      bool IField.IsVisible { get ; set ; }
      int IField.DisplayWidth { get ; set ; }
   }
}
