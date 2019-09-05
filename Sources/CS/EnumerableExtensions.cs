
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if XF
using Xamarin.Forms;
#endif

public static class EnumerableExtensions
{
   /// <summary>
   /// Returns the index of the specified object in the collection.
   /// </summary>
   /// <param name="self">The self.</param>
   /// <param name="obj">The object.</param>
   /// <returns>If found returns index otherwise -1</returns>
   public static int IndexOf(this IEnumerable self, object obj)
   {
      int index = -1;

      var enumerator = self.GetEnumerator();
      enumerator.Reset();
      int i = 0;

      while (enumerator.MoveNext())
      {
         if (enumerator.Current.Equals(obj))
         {
            index = i;
            break;
         }

         i++;
      }

      return index;
   }

#if !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4
   public static IEnumerable<DataRow> EnumerateRows(this DataTable table)
   {
      foreach (DataRow row in table.Rows)
      {
         yield return row;
      }
   }
#endif
}