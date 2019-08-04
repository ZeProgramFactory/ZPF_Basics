using System;
using System.Collections;
using System.Linq;
//using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Data;


namespace ZPF
{
   /// <summary>
   /// 11/05/17 - ME  - Add: IgnoreIntTypeAttribute
   /// 03/08/17 - ME  - Optimisation: CopyDataRowValues
   /// 02/09/17 - ME  - Enhancement: CopyDataRowValues
   /// 12/09/17 - ME  - Enhancement: CopyPropertyValues (exception message)
   /// 14/01/18 - ME  - NETSTANDARD
   /// 06/03/18 - ME  - destProperty System.Object
   /// 01/11/18 - ME  - System.DateTime <-- System.String
   /// 
   /// 2005..2018 ZePocketForge.com, SAS ZPF
   /// </summary>
   public static class ObjectExtensions
   {
#if WINCE
      public static void CopyPropertyValues(this object destination, object source)
      {
         bool OverrideTypeCheck = false;

         CopyPropertyValues(destination, source, OverrideTypeCheck);
      }

      public static void CopyPropertyValues(this object destination, object source, bool OverrideTypeCheck)
      {
#else
      public static void CopyPropertyValues(this object destination, object source, bool OverrideTypeCheck = false)
      {
#endif
         if (!OverrideTypeCheck)
         {
            if (!(destination.GetType().Equals(source.GetType())))
               throw new ArgumentException(string.Format("Type mismatch ({0}->{1})", destination.GetType().ToString(), source.GetType().ToString()));
         };

         if (destination is IEnumerable)
         {
            var dest_enumerator = (destination as IEnumerable).GetEnumerator();
            var src_enumerator = (source as IEnumerable).GetEnumerator();

            while (dest_enumerator.MoveNext() && src_enumerator.MoveNext())
               dest_enumerator.Current.CopyPropertyValues(src_enumerator.Current);
         }
         else
         {
#if WINCE
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
#else
            var destProperties = destination.GetType().GetRuntimeProperties();

            foreach (var sourceProperty in source.GetType().GetRuntimeProperties())
            {
#endif

               foreach (var destProperty in destProperties)
               {
#if WINCE
                  if (destProperty.Name == sourceProperty.Name
                      && sourceProperty.CanWrite
                      && destProperty.PropertyType.GetType()
                          .IsAssignableFrom(sourceProperty.PropertyType.GetType()))
#else
                  if (destProperty.Name == sourceProperty.Name
                      && sourceProperty.CanWrite
                      && destProperty.PropertyType.GetTypeInfo()
                          .IsAssignableFrom(sourceProperty.PropertyType.GetTypeInfo()))
#endif
                  {
                     if (sourceProperty.GetIndexParameters().Length > 0)
                     {
                        // Debug.WriteLine("!!!" + destProperty.Name + " * " + sourceProperty.PropertyType.GetTypeInfo());

                        // destProperty.SetValue(destination, sourceProperty.GetValue(source, destProperty.GetIndexParameters()), destProperty.GetIndexParameters());
                     }
                     else
                     {
                        try
                        {
                           destProperty.SetValue(destination, sourceProperty.GetValue(source, new object[] { }), new object[] { });
                        }
                        catch (Exception ex)
                        {
                           Debug.WriteLine(ex);

                           if (Debugger.IsAttached)
                           {
                              // Debugger.Break();
                           };
                        };
                     };

                     break;
                  }
               }
            }
         }
      }


      public static void CopyDataRowValues(this object destination, DataRow source, bool IgnoreCase = false, bool ToUniversalTime = false )
      {
         if (destination is IEnumerable)
         {
            //var dest_enumerator = (destination as IEnumerable).GetEnumerator();
            //var src_enumerator = (source as IEnumerable).GetEnumerator();

            //while (dest_enumerator.MoveNext() && src_enumerator.MoveNext())
            //   dest_enumerator.Current.CopyPropertyValues(src_enumerator.Current);
         }
         else
         {
#if WINCE
            var destProperties = destination.GetType().GetProperties();
#else
            var destProperties = destination.GetType().GetRuntimeProperties();
#endif

            int Ind = 0;
            foreach (DataColumn sourceFields in source.Table.Columns)
            {
               foreach (var destProperty in destProperties)
               {
                  bool DoIt = (destProperty.Name == sourceFields.ColumnName && destProperty.CanWrite);
                  //bool IgnoreIntType = ( destProperty.GetCustomAttributes().Where(x => x.ToString().Contains("IgnoreIntType")).Count() > 0 );
                  //bool IgnoreBoolType = (destProperty.GetCustomAttributes().Where(x => x.ToString().Contains("IgnoreBoolType")).Count() > 0);

                  //if (destProperty.Name == "UpdatedOn")
                  //{

                  //};

                  if (IgnoreCase)
                  {
                     DoIt = (destProperty.Name.ToLower() == sourceFields.ColumnName.ToLower() && destProperty.CanWrite);
                  };

                  if (DoIt)
                  {
                     if (source.ItemArray[Ind].GetType().FullName == "System.DateTime")
                     {
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind], null);
#else
                        var dt = (DateTime)(source.ItemArray[Ind]);

                        if (ToUniversalTime)
                        {
                           dt = (dt != DateTime.MinValue ? dt.ToLocalTime() : DateTime.MinValue);
                        };

                        destProperty.SetValue(destination, dt);
#endif
                     }
                     else if (destProperty.PropertyType.FullName == source.ItemArray[Ind].GetType().FullName)
                     {
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind], null);
#else
                        destProperty.SetValue(destination, source.ItemArray[Ind]);
#endif
                     }
                     else if (destProperty.PropertyType.FullName == "System.Object")
                     {
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind], null);
#else
                        destProperty.SetValue(destination, source.ItemArray[Ind]);
#endif
                     }
                     else if (destProperty.PropertyType.FullName.StartsWith("System.Boolean")
                              && (destProperty.GetCustomAttributes().Where(x => x.ToString().Contains("IgnoreBoolType")).Count() > 0)
                              && source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int"))
                     {
                        destProperty.SetValue(destination, Int64.Parse(source.ItemArray[Ind].ToString()) > 0);
                     }
                     else if (destProperty.PropertyType.FullName.StartsWith("System.Int")
                              && (destProperty.GetCustomAttributes().Where(x => x.ToString().Contains("IgnoreIntType")).Count() > 0)
                              && source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int"))
                     {
                        // Hmmm, null
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind], null);
#else
                        if (destProperty.PropertyType.FullName == "System.Int16")
                        {
                           destProperty.SetValue(destination, Int16.Parse(source.ItemArray[Ind].ToString()));
                        }
                        else if (destProperty.PropertyType.FullName == "System.Int32")
                        {
                           destProperty.SetValue(destination, Int32.Parse(source.ItemArray[Ind].ToString()));
                        }
                        else if (destProperty.PropertyType.FullName == "System.Int64")
                        {
                           destProperty.SetValue(destination, Int64.Parse(source.ItemArray[Ind].ToString()));
                        }
                        else
                        {
                           destProperty.SetValue(destination, source.ItemArray[Ind]);
                        };
#endif
                     }
                     else if (destProperty.PropertyType.FullName.StartsWith("System.Decimal")
                              && (source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int") || (source.ItemArray[Ind].GetType().FullName.StartsWith("System.UInt"))))
                     {
                        // Hmmm, null
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind], null);
#else
                        destProperty.SetValue(destination, Decimal.Parse(source.ItemArray[Ind].ToString()));
#endif
                     }
                     else if (source.ItemArray[Ind].GetType().FullName == "System.DBNull")
                     {
                        // Hmmm, null
#if WINCE
                        destProperty.SetValue(destination, null, null);
#else
                        destProperty.SetValue(destination, null);
#endif
                     }
                     else if (destProperty.PropertyType.FullName == "System.String"
                              && source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int"))
                     {
#if WINCE
                        destProperty.SetValue(destination, source.ItemArray[Ind].ToString(), null);
#else
                        destProperty.SetValue(destination, source.ItemArray[Ind].ToString());
#endif
                     }
                     else if (destProperty.PropertyType.FullName == "System.TimeSpan"
                              && source.ItemArray[Ind].GetType().FullName == "System.Int64")
                     {
#if WINCE
                        destProperty.SetValue(destination, TimeSpan.FromTicks(Int64.Parse(source.ItemArray[Ind].ToString())), null);
#else
                        destProperty.SetValue(destination, TimeSpan.FromTicks(Int64.Parse(source.ItemArray[Ind].ToString())));
#endif
                     }
                     else if (destProperty.PropertyType.FullName == "System.DateTime"
                              && source.ItemArray[Ind].GetType().FullName == "MySql.Data.Types.MySqlDateTime")
                     {
                        try
                        {
#if WINCE
                           destProperty.SetValue(destination, DateTime.Parse(source.ItemArray[Ind].ToString()), null);
#else
                           destProperty.SetValue(destination, DateTime.Parse(source.ItemArray[Ind].ToString()));
#endif
                        }
                        catch
                        {
                           // Hmmm, bug !!!
#if WINCE
                           destProperty.SetValue(destination, null, null);
#else
                           destProperty.SetValue(destination, null);
#endif
                        };
                     }
                     else if (destProperty.PropertyType.FullName == "System.DateTime"
                              && source.ItemArray[Ind].GetType().FullName == "System.String")
                     {
                        Debug.WriteLine(string.Format("CopyDataRowValues: Oups: {0}: {1} <-- {2}", destProperty.Name, destProperty.PropertyType.FullName, source.ItemArray[Ind].GetType().FullName));

                        try
                        {
#if WINCE
                           destProperty.SetValue(destination, DateTime.Parse(source.ItemArray[Ind].ToString()), null);
#else
                           var dt = DateTime.Parse(source.ItemArray[Ind].ToString());

                           if (dt == DateTime.MinValue || (dt - DateTime.MinValue).TotalHours == 1 || (dt - DateTime.MinValue).TotalHours == 2)
                           {
                              destProperty.SetValue(destination, null);
                           }
                           else
                           {
                              destProperty.SetValue(destination, dt);
                           };
#endif
                        }
                        catch
                        {
                           // Hmmm, bug !!!
#if WINCE
                           destProperty.SetValue(destination, null, null);
#else
                           destProperty.SetValue(destination, null);
#endif
                        };
                     }
                     else if (destProperty.PropertyType.FullName == "System.Double"
                             && (source.ItemArray[Ind].GetType().FullName == "System.Decimal" || source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int")))
                     {
#if WINCE
                        destProperty.SetValue(destination, Double.Parse(source.ItemArray[Ind].ToString()), null);
#else
                        destProperty.SetValue(destination, Double.Parse(source.ItemArray[Ind].ToString()));
#endif
                     }
                     else if (destProperty.PropertyType.FullName == "System.Boolean"
                             && source.ItemArray[Ind].GetType().FullName.StartsWith("System.UInt"))
                     {
#if WINCE
                        destProperty.SetValue(destination, UInt64.Parse(source.ItemArray[Ind].ToString()) > 0, null);
#else
                        destProperty.SetValue(destination, UInt64.Parse(source.ItemArray[Ind].ToString()) > 0);
#endif
                     }
                     else if (destProperty.PropertyType.BaseType.ToString() == "System.Enum"
                              && source.ItemArray[Ind].GetType().FullName.StartsWith("System.Int"))
                     {
#if WINCE
                        destProperty.SetValue(destination, UInt64.Parse(source.ItemArray[Ind].ToString()), null);
#else
                        destProperty.SetValue(destination, Enum.Parse(destProperty.PropertyType, source.ItemArray[Ind].ToString()));
#endif
                     }
                     else
                     {
                        Debug.WriteLine(string.Format("CopyDataRowValues: {0}: {1} <-- {2}", destProperty.Name, destProperty.PropertyType.FullName, source.ItemArray[Ind].GetType().FullName));

                        //if (Debugger.IsAttached)
                        //{
                        //   Debugger.Break();
                        //};
                     };
                     break;
                  }
               }

               Ind++;
            }
         }
      }

   }
}
