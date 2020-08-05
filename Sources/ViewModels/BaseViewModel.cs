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

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
