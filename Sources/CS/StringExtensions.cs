using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZPF
{
   public static class StringExtensions
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      /// Returns a string containing a specified number of characters from the left side of a string.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="Nb"></param>
      /// <returns></returns>
      public static string Left(this string source, int Nb)
      {

         string Result = source;

         if (string.IsNullOrEmpty(source))
         {
            return source;
         };

         if (Nb < 0)
         {
            int l = Result.Length + Nb;
            if (l < 0) l = 0;

            Result = Result.Substring(0, l);
         }
         else
         {
            if (source.Length > Nb)
            {
               Result = Result.Substring(0, Nb);
            };
         };

         return Result;
      }

      /// <summary>
      /// Returns a string containing a specified number of characters from the right side of a string.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="Nb"></param>
      /// <returns></returns>
      public static string Right(this string source, int Nb)
      {
         string Result = source;

         if (string.IsNullOrEmpty(source))
         {
            return source;
         };

         if (source.Length > Nb)
         {
            Result = Result.Substring(source.Length - Nb);
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      /// Returns the specified number first lines of a text.
      /// </summary>
      /// <param name="source"></param>
      /// <param name="Nb"></param>
      /// <returns></returns>
      public static string Top(this string source, int Nb)
      {
         if (string.IsNullOrEmpty(source))
         {
            return source;
         };

         string Result = "";

         var list = new TStrings();
         list.Text = source;

         for( int i=0; i < Nb && i < list.Count; i++)
         {
            Result = Result + list[i] + Environment.NewLine;
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      /// Returns the first line of a text.
      /// </summary>
      /// <param name="source"></param>
      /// <returns></returns>
      public static string FirstLine(this string source)
      {
         if (string.IsNullOrEmpty(source))
         {
            return source;
         };

         if (source.IndexOf("\r") != -1)
         {
            return source.Substring(0, source.IndexOf("\r"));
         };

         return source;
      }

      public static string RemoveChars(this string source, string chars)
      {
         if (string.IsNullOrEmpty(source))
         {
            return source;
         };

         if (string.IsNullOrEmpty(chars))
         {
            return source;
         };

         for (int i = 0; i < chars.Length; i++)
         {
            source = source.Replace( chars[i]+"", "");
         };

         return source;
      }

      public static string RemoveSpecialCharacters(this string str)
      {
         StringBuilder sb = new StringBuilder();
         foreach (char c in str)
         {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            {
               sb.Append(c);
            }
         }
         return sb.ToString();
      }

      public static string ReplaceFirstOccurrence(this string source, string find, string replace)
      {
         int place = source.IndexOf(find);

         if (place<0)
         {
            return source;
         }

         string result = source.Remove(place, find.Length).Insert(place, replace);

         return result;
      }

      public static string ReplaceLastOccurrence(this string source, string find, string replace)
      {
         int place = source.LastIndexOf(find);

         if (place < 0)
         {
            return source;
         }
         string result = source.Remove(place, find.Length).Insert(place, replace);

         return result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static int IndexOfTStrings(this string source, TStrings List)
      {
         if (string.IsNullOrEmpty(source))
         {
            return -1;
         };

         if (List.Count == 0)
         {
            return -1;
         };

         for (int i = 0; i < List.Count; i++)
         {
            string chars = List[i];

            if (source.IndexOf(chars) > 0)
            {
               return source.IndexOf(chars);
            }
         }
         return -1;
      }

      public static int LastIndexOfTStrings(this string source, TStrings List)
      {
         if (string.IsNullOrEmpty(source))
         {
            return -1;
         };

         if (List.Count==0)
         {
            return -1;
         };

         for (int i = 0; i < List.Count; i++)
         {
            string chars = List[i];
          
            if (source.LastIndexOf(chars) > 0)
            {
               return source.LastIndexOf(chars);
            }
         }

         return -1;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static string Clean(this string source )
      {
         if (string.IsNullOrEmpty(source)) return source;

         return source.Trim(new char[] { ' ', '\t', '\n', '\r' });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      ///   SqlLike with wildcards used in conjunction with the LIKE operator:
      ///      % - The percent sign represents zero, one, or multiple characters
      ///      _ - The underscore represents a single character
      ///   And as in SQLServer      
      ///      [charlist] - Defines sets and ranges of characters to match
      ///      [^ charlist] - Defines sets and ranges of characters NOT to match
      /// </summary>
      /// <param name="source"></param>
      /// <param name="pattern"></param>
      /// <returns></returns>
      public static bool SqlLike(this string source, string pattern)
      {
         bool isMatch = true,
             isWildCardOn = false,
             isCharWildCardOn = false,
             isCharSetOn = false,
             isNotCharSetOn = false,
             endOfPattern = false;
         int lastWildCard = -1;
         int patternIndex = 0;

         List<char> set = new List<char>();
         char p = '\0';

         for (int i = 0; i < source.Length; i++)
         {
            char c = source[i];
            endOfPattern = (patternIndex >= pattern.Length);
            if (!endOfPattern)
            {
               p = pattern[patternIndex];

               if (!isWildCardOn && p == '%')
               {
                  lastWildCard = patternIndex;
                  isWildCardOn = true;
                  while (patternIndex < pattern.Length &&
                      pattern[patternIndex] == '%')
                  {
                     patternIndex++;
                  }
                  if (patternIndex >= pattern.Length) p = '\0';
                  else p = pattern[patternIndex];
               }
               else if (p == '_')
               {
                  isCharWildCardOn = true;
                  patternIndex++;
               }
               else if (p == '[')
               {
                  if (pattern[++patternIndex] == '^')
                  {
                     isNotCharSetOn = true;
                     patternIndex++;
                  }
                  else isCharSetOn = true;

                  set.Clear();
                  if (pattern[patternIndex + 1] == '-' && pattern[patternIndex + 3] == ']')
                  {
                     char start = char.ToUpper(pattern[patternIndex]);
                     patternIndex += 2;
                     char end = char.ToUpper(pattern[patternIndex]);
                     if (start <= end)
                     {
                        for (char ci = start; ci <= end; ci++)
                        {
                           set.Add(ci);
                        }
                     }
                     patternIndex++;
                  }

                  while (patternIndex < pattern.Length &&
                      pattern[patternIndex] != ']')
                  {
                     set.Add(pattern[patternIndex]);
                     patternIndex++;
                  }
                  patternIndex++;
               }
            }

            if (isWildCardOn)
            {
               if (char.ToUpper(c) == char.ToUpper(p))
               {
                  isWildCardOn = false;
                  patternIndex++;
               }
            }
            else if (isCharWildCardOn)
            {
               isCharWildCardOn = false;
            }
            else if (isCharSetOn || isNotCharSetOn)
            {
               bool charMatch = (set.Contains(char.ToUpper(c)));
               if ((isNotCharSetOn && charMatch) || (isCharSetOn && !charMatch))
               {
                  if (lastWildCard >= 0) patternIndex = lastWildCard;
                  else
                  {
                     isMatch = false;
                     break;
                  }
               }
               isNotCharSetOn = isCharSetOn = false;
            }
            else
            {
               if (char.ToUpper(c) == char.ToUpper(p))
               {
                  patternIndex++;
               }
               else
               {
                  if (lastWildCard >= 0) patternIndex = lastWildCard;
                  else
                  {
                     isMatch = false;
                     break;
                  }
               }
            }
         }

         endOfPattern = (patternIndex >= pattern.Length);

         if (isMatch && !endOfPattern)
         {
            bool isOnlyWildCards = true;
            for (int i = patternIndex; i < pattern.Length; i++)
            {
               if (pattern[i] != '%')
               {
                  isOnlyWildCards = false;
                  break;
               }
            }

            if (isOnlyWildCards) endOfPattern = true;
         }

         return isMatch && endOfPattern;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}


