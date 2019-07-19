using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Diagnostics;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

using System.Threading.Tasks;

namespace ZPF
{
   public class BaseViewModel : INotifyPropertyChanged
   {
      public BaseViewModel()
      {

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      #region INotifyPropertyChanged

      // - - - INotifyPropertyChanged Members - - - 

      public event PropertyChangedEventHandler PropertyChanged;

      public void ClearPropertyChanged()
      {
         foreach (Delegate d in PropertyChanged.GetInvocationList())
         {
            PropertyChanged -= (PropertyChangedEventHandler)d;
         }
      }

      protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         var handler = PropertyChanged;
         if (handler != null)
         {
            try
            {
               handler(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
               Debug.WriteLine(string.Format("BaseViewModel.OnPropertyChanged({0})", propertyName) + Environment.NewLine + ex.Message);

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
            };
         }
      }

      //protected bool OnPropertyChanged( ref object prop, object value, [CallerMemberName] string propertyName = null )
      //{
      //   if (GetPropValue(this, propertyName) != value)
      //   {
      //      prop = value;
      //      // SetPropValue(this, propertyName, value);

      //      var handler = PropertyChanged;
      //      if (handler != null)
      //      {
      //         handler(this, new PropertyChangedEventArgs(propertyName));
      //      }

      //      return true;
      //   }
      //   else
      //   {
      //   };

      //   return false;
      //}

      //public static object GetPropValue(object src, string propName)
      //{
      //   return src.GetType().GetProperty(propName).GetValue(src, null);
      //}

      //public static void SetPropValue(object src, string propName, object value)
      //{
      //   src.GetType().GetProperty(propName).SetValue( src, value);
      //}

      #endregion

      protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
      {
         if (propertyName != null)
         {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;

            try
            {
               OnPropertyChanged(propertyName);
            }
            catch (Exception ex)
            {
               Debug.WriteLine(string.Format("BaseViewModel.SetField({0})", propertyName) + Environment.NewLine + ex.Message);

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
            };
            return true;
         }
         else
         {
            return false;
         };
      }

      protected bool SetFieldNN<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
      {
         if (value != null)
         {
            return SetField(ref field, value, propertyName);
         }
         else
         {
            return false;
         };
      }

//#if !PCL && !WebService && !WINCE && !IOT && !NETCOREAPP1_0 && !NETSTANDARD2_0
//      private DependencyObject _DependencyObject = null;
//      [ZPF.DB_Attributes.Ignore]
//      protected bool IsInDesignMode
//      {
//         get
//         {
//            if (_DependencyObject == null) _DependencyObject = new DependencyObject();
//            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(_DependencyObject);
//         }
//      }
//#endif

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
