﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPF
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - 

   public enum Orientations { Portrait, Landscape }
   public enum PageSizes { Custom, A4 }

   public enum HAlignments { Left, Center, Right, Justify, None }
   public enum VAlignments { Top, Center, Bottom, None }
   public enum LineStyles { None, Hair, Thin, Medium }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - 

   public class ColDef
   {
      public ColDef()
      {
      }

      public ColDef(string name, string header, double width)
      {
         Name = name;
         Header = header;

         Width = width;
         NbChars = ((int)width != width ? NbChars : (int)width);
      }

      public ColDef(string name, string header, double width, HAlignments hAlignment) : this(name, header, width)
      {
         HAlignment = hAlignment;
      }

      public int Ind { get; set; }

      public string Name { get; set; }
      public string Header { get; set; }

      public int NbChars { get; set; } = -1;
      public double Width { get; set; }

      public HAlignments HAlignment { get; set; } = HAlignments.None;
      //public NumFmtIds NumFmtId { get; set; } = NumFmtIds.none;

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name} ({Ind})";
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - 

   static class Layout_Basics
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  


      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  
   }
}
