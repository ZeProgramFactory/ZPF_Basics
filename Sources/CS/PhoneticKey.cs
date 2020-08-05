using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if SqlServer
using ZPF;
using System.Globalization;
#endif

namespace ZPF
{
   /// <summary>
   /// 
   /// 26/09/12 - ME  - Creation based on ClefPhon.pas (1999);
   /// 01/10/13 - ME  - Add: IndexRecord, IndexTable
   /// 02/03/14 - ME  - Bugfix: IndexRecord(string TableName, string PKName, string PKValue, string Fields)
   /// 17/03/14 - ME  - Add: string RemoveDiacritics(string text)
   /// 07/09/17 - ChM - Ctrl IsNullOrEmpty input text in RemoveDiacritics  
   /// 21/10/17 - ME  - Split:  PhoneticKey & PhoneticKeyHelper
   ///  
   /// 2013..2017 ZePocketForge.com, SAS ZPF
   /// </summary>
   /// 
   public class PhoneticKey
   {
      static char c;
      static string tmp;

      public static string QueAlphabet(string Source, bool Numbers)
      {
         tmp = ""; // chaine r‚sultat }

         for (int i = 0; i < Source.Length; i++)
         {
            c = Source[i];  // extraction d'un caractère

            if ((c >= 'A') && (c <= 'Z'))
            {
               tmp = tmp + c;
            };

            if (Numbers && (c >= '0') && (c <= '9'))
            {
               tmp = tmp + c;
            };
         };

         return tmp;
      }


      public static string FilterDoubleChar(string source, bool Numbers)
      {
         string st = "";

         for (int i = 0; i < source.Length; i++)
         {
            if (Numbers && (source[i] >= '0') && (source[i] <= '9'))
            {
               st = st + source[i];
            }
            else
            {
               if (i + 1 == source.Length)
               {
                  st = st + source[i];
               }
               else
               {
                  if (source[i] != source[i + 1])
                  {
                     st = st + source[i];
                  };
               };
            };
         };

         return st;
      }

      public static string AlphaCode(string Source)
      {
         tmp = "";

         for (int i = 0; i < Source.Length; i++)
         {
            c = Source[i];

            if (c != 'H')
            {
               switch (c)
               {
                  case 'B': c = 'P'; break;
                  case 'C': c = 'K'; break;
                  case 'D': c = 'T'; break;
                  case 'F': c = 'S'; break;
                  case 'J': c = 'I'; break;
                  case 'M': c = 'N'; break;
                  case 'W': c = 'V'; break;
                  case 'Y': c = 'I'; break;
                  case 'Z': c = 'S'; break;
                  default:
                     // otherwise nothing to do
                     break;
               };

               tmp = tmp + c;
            };
         };

         return tmp;
      }

      public static string PlaceJoker(string Source)
      {
         for (int i = 0; i < Source.Length; i++)
         {
            c = Source[i];

            switch (c)
            {
               case 'A': c = '*'; break;
               case 'E': c = '*'; break;
               case 'I': c = '*'; break;
               case 'O': c = '*'; break;
               case 'U': c = '*'; break;
            };

            // Source[i] = c;
            char[] aTmp = Source.ToCharArray();
            aTmp[i] = c;
            Source = aTmp.ToString();
         };

         return Source;
      }

      public static string RemoveDiacritics(string text)
      {
         if (string.IsNullOrEmpty(text))
         {
            return "";
         };

         byte[] tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(text);

         return System.Text.Encoding.UTF8.GetString(tempBytes, 0, text.Length);
      }

      public static string Text2PhonKey(string Source, bool joker, bool Numbers)
      {
         string st;

         // st = UCaseASC7( source );
         st = Source.ToUpper();
         st = RemoveDiacritics(st);
         st = FilterDoubleChar(st, Numbers);

         //ToDo: adaptation linguistique
         st = st.Replace("ES ", "E ");    // Suppression du pluriel en français

         st = QueAlphabet(st, Numbers);
         st = AlphaCode(st);

         st = FilterDoubleChar(st, Numbers);

         if (joker)
         {
            st = PlaceJoker(st);
            st = FilterDoubleChar(st, Numbers);
         };

         return st;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
   }
}

