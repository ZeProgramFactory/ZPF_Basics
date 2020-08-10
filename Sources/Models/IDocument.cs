
using System;

namespace ZPF
{
   public interface IDocument
   {
      /// <summary>
      /// GUID for everyone
      /// </summary>
      string GUID { get; set; }

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

      /// <summary>
      /// Document full path (at least fileName with extension)
      /// </summary>
      string FullPath { get; set; }

      /// <summary>
      /// DateTime >= Creation 
      /// </summary>
      DateTime UpdatedOn { get; set; }

      // - - -  - - - 

      IDocument Copy();
   }
}
