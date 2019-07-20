
using System;

#if WPF || NETSTANDARD2_0
using System.Windows;
using System.Threading.Tasks;
#endif

#if XF
using Xamarin.Forms;
using ZPF.XF;
#endif

namespace ZPF
{
   /// <summary>
   /// DoIt ...
   /// 
   /// 25/05/17 - ME  - Creation ...
   /// 29/05/17 - ME  - WPF
   /// 31/05/17 - ME  - add: static bool OnBackground(action)
   /// 31/05/17 - ME  - WPF: static void Delay(int millisecondsDelay, Action action)
   /// 20/03/18 - ME  - WinCE: static void Delay(int millisecondsDelay, Action action)
   /// 30/11/18 - ME  - NETCOREAPP2_0
   /// 07/06/19 - ME  - Bugfix: OnMainThread: NETCOREAPP2_0
   /// 
   /// 2017..2019 SAS ZPF
   /// </summary>
   public static class DoIt
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      public static void OnBackground(Action action)
      {
         if (action == null) return;

#if XF || WINDOWS_UWP || STD || WPF || WebService || NETCOREAPP2_0 || NETSTANDARD2_0
         System.Threading.Tasks.Task.Factory.StartNew(() =>
         {
            action();
         });
#endif

#if WINCEl
         Delay(1, action);
#endif
      }

#if WPF || WebService || NETSTANDARD2_0
      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      /// <param name="callback"></param>
      public static void OnBackground(Action action, Action<object> callback)
      {
         System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
         worker.RunWorkerCompleted += (object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) =>
         {
            callback(sender);
         };
         worker.DoWork += (object sender, System.ComponentModel.DoWorkEventArgs e) =>
         {
            action();
         };
         worker.RunWorkerAsync();
      }
#endif

      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      /// <returns></returns>
      public static bool OnBackground(Func<bool> action)
      {
         if (action == null) return false;

#if XF || WINDOWS_UWP || STD || WPF || WebService || NETCOREAPP2_0 || NETSTANDARD2_0
         System.Threading.Tasks.Task.Factory.StartNew(() =>
         {
            return action();
         });
#endif

         return false;
      }

#if WINDOWS_UWP
      public static async void OnMainThread(Action action)
      {
         await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
         {
            action();
         });
      }
#else

      /// <summary>
      ///
      /// </summary>
      /// <param name="action"></param>
      public static void OnMainThread(Action action)
      {
         if (action == null) return;

#if XF
         Device.BeginInvokeOnMainThread(() =>
         {
            action();
         });

         return;
#endif

#if WPF
         Application.Current.Dispatcher.Invoke(new Action(() =>
         {
            action();
         }));

         return;
#endif

#if NETSTANDARD2_0
         //System.Threading.Tasks.D Dispatcher.
         //Invoke(new Action(() =>
         //{
         action();
         //}));
#endif

      }
#endif

      /// <summary>
      /// Creates a task that completes after a time delay.
      /// </summary>
      /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task, or -1 to wait indefinitely.</param>
      /// <param name="action"></param>
      public static void Delay(int millisecondsDelay, Action action)
      {
         if (action == null) return;
         if (millisecondsDelay < 1) return;

#if XF|| WINDOWS_UWP || STD || WPF || WebService || NETCOREAPP2_0 || NETSTANDARD2_0
         var task = System.Threading.Tasks.Task.Delay(millisecondsDelay).ContinueWith(t => action());
#endif

#if WINCE
         System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
         t.Interval = millisecondsDelay;
         t.Tick += (object _sender, EventArgs _e) =>
         {
            t.Enabled = false;

            action();
         };
         t.Enabled = true;
#endif
      }

      /// <summary>
      /// ???? Creates a task that completes after a time delay. ????
      /// </summary>
      /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task, or -1 to wait indefinitely.</param>
      /// <param name="action"></param>
#if !WINDOWS_UWP
#if XF || WINDOWS_UWP || STD
#if NETSTANDARD2_0

      public static System.Timers.Timer Timeout(System.Timers.Timer _Timer, int millisecondsDelay, Action action)
      {
         if (action == null) return null;
         if (millisecondsDelay < 1) return null;

         //ToDo ¤ !!! timer
         if (_Timer != null)
         {
            _Timer.Stop();
         };

         _Timer = new System.Timers.Timer(millisecondsDelay);
         _Timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
#else

      public static PCLTimer Timeout(PCLTimer _Timer, int millisecondsDelay, Action action)
      {
         if (_Timer != null)
         {
            _Timer.Stop();
         };

         _Timer = new PCLTimer();
         _Timer.Interval = millisecondsDelay;
         _Timer.Elapsed += (object sender, EventArgs e) =>
#endif
         {
            _Timer.Stop();
            action();
         };

         _Timer.Start();

         return _Timer;
      }
#endif
#endif
   }
}
