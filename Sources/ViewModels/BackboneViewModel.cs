using System;
using System.Runtime.CompilerServices;
using ZPF;
using System.Diagnostics;
using ZPF.AT;

/// <summary>
/// 14/01/18 - ME  - NETSTANDARD1_3
/// 01/02/18 - ME  - --> Std: ZPF_Basics
/// 22/01/25 - ME  - IsNotBusy
/// 21/02/25 - ME  - --> "BaseViewModel2"
/// 
/// 2005..2025 ZePocketForge.com, SAS ZPF, ZeProgFactory
/// </summary>
public class BackboneViewModel : BaseViewModel<BackboneViewModel>
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public enum MessageBoxType { Info, Warning, Error, Confirmation }

   public void MessageBox(Action<string> msgBoxCallBack, MessageBoxType confirmation, string v)
   {
      throw new NotImplementedException();
   }

   public bool MessageBox(MessageBoxType type, string Text)
   {
      return MessageBox(type, Text, "");
   }

   public bool MessageBox(MessageBoxType type, string Text, string Caption)
   {
      try
      {
         if (_MsgCallBack != null)
         {
            return _MsgCallBack(type, Text, Caption);
         }
         else
         {
            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
         };
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex.Message);

         if (Debugger.IsAttached)
         {
            Debugger.Break();
         };
      };

      return false;
   }

   public bool MessageBox(string Text)
   {
      return MessageBox(MessageBoxType.Info, Text);
   }

   Func<MessageBoxType, string, string, bool> _MsgCallBack = null;
   public void InitMsgCallBack(Func<MessageBoxType, string, string, bool> MsgCallBack)
   {
      _MsgCallBack = MsgCallBack;
      // AuditTrailViewModel.Current.InitMsgCallBack(MsgCallBack);
   }

   public void MsgReponse(string Response)
   {
      throw new NotImplementedException();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   Action _DoEventsCallBack = null;
   public void InitDoEventsCallBack(Action DoEventsCallBack)
   {
      _DoEventsCallBack = DoEventsCallBack;
   }

   public void DoEvents()
   {
      if (_DoEventsCallBack != null)
      {
         _DoEventsCallBack();
      }
      else
      {
         if (Debugger.IsAttached)
         {
            Debugger.Break();
         };
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   /// <summary>
   /// Gets or sets a value indicating whether the application is busy.
   /// </summary>
   public bool IsBusy
   {
      get { return _IsBusy; }
      set
      {
         if (SetField(ref _IsBusy, value))
         {
            _BusyCounter = (_IsBusy ? _BusyCounter : 0);

            OnPropertyChanged("IsNotBusy");

            if (_DoEventsCallBack != null)
            {
               _DoEventsCallBack();
            };
         };
      }
   }

   bool _IsBusy = false;

   /// <summary>
   /// Gets a value indicating whether the application is not busy.
   /// </summary>
   public bool IsNotBusy
   {
      get { return ! _IsBusy; }
      set
      {
         IsBusy = !value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public string DataPath { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   string _BusyTitle = "working ...";
   public string BusyTitle
   {
      get { return _BusyTitle; }
      set
      {
         if (SetField(ref _BusyTitle, value))
         {
            Log.Write(new AuditTrail { Level = ErrorLevel.Log, Message = _BusyTitle });
         };
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   string _BusySubTitle = "";
   public string BusySubTitle
   {
      get { return _BusySubTitle; }
      set
      {
         string Prev = _BusySubTitle;

         if (SetField(ref _BusySubTitle, value))
         {
            if (!string.IsNullOrEmpty(Prev))
            {
               Log.Write(new AuditTrail { Level = ErrorLevel.Log, Message = _BusySubTitle });

               _BusyHistory.Add(Prev);
               while (_BusyHistory.Count > 7) _BusyHistory.Delete(0);

               OnPropertyChanged("BusyHistory");
            };
         };
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public object MainWindow { get; set; }

   int _BusyCounter = 0;

   private TStrings _BusyHistory = new TStrings();
   public String BusyHistory { get => _BusyHistory.Text; set { _BusyHistory.Text = value; OnPropertyChanged(); } }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public bool Silent
   {
      get => _Silent;
      set
      {
         _Silent = value;
         _IsBusy = !Silent && (_BusyCounter > 0);
         OnPropertyChanged("IsBusy");
      }
   }

   bool _Silent = false;

   /// <summary>
   /// Increment the busy counter.
   /// </summary>
   /// <param name="callerMemberName"></param>
   public void IncBusy([CallerMemberName] string callerMemberName = null)
   {
      _BusyCounter++;
      IsBusy = !Silent && (_BusyCounter > 0);

      Debug.WriteLine("IncBusy " + callerMemberName);
   }

   public void DecBusy([CallerMemberName] string callerMemberName = null)
   {
      _BusyCounter--;
      IsBusy = !Silent && (_BusyCounter > 0);

      if (_BusyCounter < 0) _BusyCounter = 0;

      if (!IsBusy)
      {
         BusyTitle = "working ...";
         BusySubTitle = "";
         BusyHistory = "";
      };

      Debug.WriteLine("DecBusy " + callerMemberName);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   private string _CurrentRef = "";

   public string CurrentRef
   {
      get { return _CurrentRef; }
      set { SetField(ref _CurrentRef, value); }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public void Navigate(string Ref)
   {
      CurrentRef = Ref;

      if (NavigationCallBack != null)
      {
         NavigationCallBack(Ref);
      }
   }

   Action<string> _NavigationCallBack = null;

   public Action<string> NavigationCallBack
   {
      get { return _NavigationCallBack; }
      set { _NavigationCallBack = value; }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}

