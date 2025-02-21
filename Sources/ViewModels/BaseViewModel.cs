using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace ZPF;

/// <summary>
/// Base class for ViewModel classes
/// <code>
/*
public class yourViewModel : BaseViewModel2<yourViewModel>
{
   public ICommand SimpleCommand { get; } 

   public yourViewModel()
   {
      SimpleCommand = new RelayCommand(ExecuteSimpleCommand, CanExecuteSimpleCommand);
   }

   public string Hello()
   {
      return "Hello World!";
   }

   public string Toto { get => _toto; set => SetField(ref _toto, value); }
   string _toto = "toto";


   private void ExecuteSimpleCommand(object parameter)
   {
      // Your command execution logic here
      Console.WriteLine("SimpleCommand executed with parameter: " + parameter);
   }

   private bool CanExecuteSimpleCommand(object parameter)
   {
      // Your logic to determine if the command can execute
      return true; // or some condition
   }
}


public class yourClass
{
   public void yourCode()
   {
      yourViewModel.Current.Hello();

      yourViewModel.Current.Toto = "titi";
   }
}
 */
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseViewModel<T> : ObservableObject where T : class
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   #region Singleton 

   private static readonly Lazy<T> sCurrent = new Lazy<T>(() => CreateInstanceOfT());

   public static T Current { get { return sCurrent.Value; } }

   private static T CreateInstanceOfT()
   {
      return Activator.CreateInstance(typeof(T), true) as T;
   }

   #endregion

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   #region RelayCommand
   public class RelayCommand : ICommand
   {
      private readonly Action<object> _execute;
      private readonly Predicate<object> _canExecute;

      public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
      {
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         _canExecute = canExecute;
      }

      public bool CanExecute(object parameter)
      {
         return _canExecute == null || _canExecute(parameter);
      }

      public void Execute(object parameter)
      {
         _execute(parameter);
      }

      public event EventHandler CanExecuteChanged;

      //// System.Windows.Input - PresentationCore.dll
      //public event EventHandler CanExecuteChanged
      //{
      //   add { CommandManager.RequerySuggested += value; }
      //   remove { CommandManager.RequerySuggested -= value; }
      //}
   }

   #endregion

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}



