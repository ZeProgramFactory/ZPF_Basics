using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZPF;


/// <summary>
/// The ObservableObject is a base class for objects that need to be observed. 
/// By implementing the INotifyPropertyChanged and INotifyPropertyChanging interfaces, 
/// it becomes a versatile starting point for any object requiring property change notifications.
/// </summary>
public class ObservableObject : INotifyPropertyChanged
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

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

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   #region SetField

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
            }
         }

         return true;
      }
      else
      {
         return false;
      }
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
      }
   }

   #endregion

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}
