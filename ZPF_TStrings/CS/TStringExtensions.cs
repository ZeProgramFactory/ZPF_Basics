namespace ZPF
{
   public static class TStringExtensions
   {
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

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static int LastIndexOfTStrings(this string source, TStrings List)
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

            if (source.LastIndexOf(chars) > 0)
            {
               return source.LastIndexOf(chars);
            }
         }

         return -1;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}


