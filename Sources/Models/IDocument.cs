
using System;

namespace ZPF
{
   public interface IDocument
   {
      /// <summary>
      /// Primary Key - GUID for everyone
      /// </summary>
      string PK { get; set; }

      /// <summary>
      /// Attached to a record of 'ExtType'
      /// </summary>
      string ExtType { get; set; }

      /// <summary>
      /// Foreign key for the record of 'ExtType'
      /// </summary>
      string ExtRef { get; set; }

      /// <summary>
      /// Title
      /// </summary>
      string Title { get; set; }

      public string FileName { get; set; }

      /// <summary>
      /// Document full path (at least fileName with extension)
      /// </summary>
      string FullPath { get; set; }

      long FileSize { get; set; }

      DateTime FileDate { get; set; }

      /// <summary>
      /// DateTime >= Creation 
      /// </summary>
      DateTime UpdatedOn { get; set; }

      string UpdatedBy { get; set; }

      // - - -  - - - 

      IDocument Copy();
   }
}
