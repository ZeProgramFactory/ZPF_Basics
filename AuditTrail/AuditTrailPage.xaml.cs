using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZPF.AT;
using ZPF.SQL;

#if REPORTING
using ZPF.Reporting;
#endif

namespace ZPF
{
   /// <summary>
   /// Purge multiniveau (48h, 1 semaine, 1 mois)
   /// Meilleure gestion du fichier TXT
   /// Lien avec user
   /// Typage fort: Module-Fonction
   /// Champs data: "Input", "Output" avec typage
   /// Redéfinition de l'interface d'appel
   /// </summary>
   public partial class AuditTrailPage : Page
   {
      AuditTrailViewModel _AuditTrailViewModel = null;

      public AuditTrailPage(AuditTrailViewModel auditTrailViewModel, string IniFileName = "")
      {
         DataContext = auditTrailViewModel;
         _AuditTrailViewModel = auditTrailViewModel;

         InitializeComponent();

         //dataGridTools = new DataGridTools(IniFileName, "AuditTrail", dataGrid);

         //var FontFamily = new FontFamily(@"./Fonts/IconFont.tft");

         //btnGo.FontFamily = ZPF.Fonts.IF.FontFamily;
         //btnGo.FontFamily = FontFamily;
         //btnGo.FontSize = 24;
         //btnGo.Content = ZPF.Fonts.IF.Filter_Standard;

         //btnClear.FontFamily = ZPF.Fonts.IF.FontFamily;
         //btnClear.FontSize = 24;
         //btnClear.Content = ZPF.Fonts.IF.Filter_Delete;

         //btnExcel.FontFamily = ZPF.Fonts.IF.FontFamily;
         //btnExcel.FontSize = 24;
         //btnExcel.Content = ZPF.Fonts.IF.Excel_Online;

         //btnPDF.FontFamily = ZPF.Fonts.IF.FontFamily;
         //btnPDF.FontSize = 24;
         //btnPDF.Content = ZPF.Fonts.IF.File_Format_PDF;

         //image.Source = ZPF.Fonts.IF.GetImageSource( ZPF.Fonts.IF.Foot_print_02[0], Brushes.Black, 512, -40);

         // - - -  - - - 

         if (DBViewModel.Current.AllTables.Where(x => x.TableName == "AuditTrail").Count() < 1)
         {
            return;
         };

         cbFilterLevel.ItemsSource = Enum.GetValues(typeof(ErrorLevel));
         cbFilterLevel.ComboBox.SelectedItem = _AuditTrailViewModel.Level;

         cbFilterTerminal.ItemsSource = DB_SQL.Query<NameValue>(DBViewModel.Current.Connection, _AuditTrailViewModel.GetTerminalSQL());
         cbFilterTerminal.ComboBox.DisplayMemberPath = "Name";
         cbFilterTerminal.ComboBox.SelectedValuePath = "Value";
         cbFilterTerminal.ComboBox.SelectedItem = _AuditTrailViewModel.TerminalID;

         cbFilterUser.ItemsSource = DB_SQL.Query<NameValue>(DBViewModel.Current.Connection, _AuditTrailViewModel.GetUserSQL());
         cbFilterUser.ComboBox.DisplayMemberPath = "Name";
         cbFilterUser.ComboBox.SelectedValuePath = "Value";
         cbFilterUser.ComboBox.SelectedItem = _AuditTrailViewModel.FKUser;

         cbFilterChilds.IsChecked = _AuditTrailViewModel.Childs;
         cbFilterDebug.IsChecked = _AuditTrailViewModel.IsDebug;

         cbFilterDebug.Visibility = (Debugger.IsAttached ? Visibility.Visible : Visibility.Collapsed);

         btnGo_Click(null, null);

         // - - -  - - - 

#if REPORTING
         var rep = new ZPF.Reporting.Report();
         rep.Title = "AuditTrail";
         rep.SubTitle = DateTime.Now.ToString();
         rep.Orientation = "LANDSCAPE";

         // Add report fields to the report object.
         rep.Fields.Add(new Field("TimeStamp", "TimeStamp", 80, ALIGN.CENTER));
         rep.Fields.Add(new Field("Level", "Level", 50));
         rep.Fields.Add(new Field("Tag", "Tag"));
         rep.Fields.Add(new Field("Message", "Message", 150));
         rep.Fields.Add(new Field("Data", "Data", 150));
         rep.Fields.Add(new Field("TerminalID", "TerminalID", 80, ALIGN.CENTER));
         rep.Fields.Add(new Field("FKUser", "FKUser"));
         rep.Fields.Add(new Field("ItemID", "ItemID"));
         rep.Fields.Add(new Field("ItemType", "ItemType"));

         // rep.Logo = "Logo.png";

         string FileName = string.Format("AuditTrail_{0}.pdf", DateTime.Now.ToString("yyyyMMdd"));
         string tempPath = System.IO.Path.GetTempFileName();

         ReportEngine report = ReportEngine.Instance;
         ReportEngine.Instance.ReportSource = DataTable2DataSet(_AuditTrailViewModel.AuditTrail.ToDataTable()); ;
         ReportingViewModel.Instance.SelectedReport = rep;

         report.CurrentReport = rep;
#endif

         frameBody.Navigate(new AuditTrail_Details_Page(_AuditTrailViewModel));

         // - - -  - - - 

         cbAutoRefresh.CheckBox.Checked += CheckBox_Checked;
         cbAutoRefresh.CheckBox.Unchecked += CheckBox_Checked;

         //ToDo: cbAutoRefresh.IsChecked = MainViewModel.Current.AutoRefresh;

         btnGo_Click(null, null);

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void CheckBox_Checked(object sender, RoutedEventArgs e)
      {
         if (cbAutoRefresh.IsChecked == true)
         {
            AutoRefresh();
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      void AutoRefresh()
      {
         // ToDo:  ¤¤ChM Add Params sur nb et tmps
         //AuditTrailViewModel.Secondary.LoadAuditTrail(DBViewModel.Current.Connection, 500);

         DoIt.Delay(10000, () =>
         {
            DoIt.OnMainThread(() =>
            {
               if (cbAutoRefresh.IsChecked == true)
               {
                  AutoRefresh();
               };
            });
         });
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void btnPurge48_Click(object sender, RoutedEventArgs e)
      {
         //if (AuditTrailViewModel.Secondary.Clean(AuditTrailViewModel.CleanRange.TwoDays))
         //{
         //   BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "OK");
         //   AuditTrailViewModel.Secondary.LoadAuditTrail();
         //};
      }

      private void btnPurge1S_Click(object sender, RoutedEventArgs e)
      {
         //if (AuditTrailViewModel.Secondary.Clean(AuditTrailViewModel.CleanRange.Week))
         //{
         //   BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "OK");
         //   AuditTrailViewModel.Secondary.LoadAuditTrail();
         //};
      }

      private void btnPurge1M_Click(object sender, RoutedEventArgs e)
      {
         //if (AuditTrailViewModel.Secondary.Clean(AuditTrailViewModel.CleanRange.Month))
         //{
         //   BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "OK");
         //   AuditTrailViewModel.Secondary.LoadAuditTrail();
         //};
      }

      private void btnReset_Click(object sender, RoutedEventArgs e)
      {
         //if (AuditTrailViewModel.Secondary.Clear())
         //{
         //   BackboneViewModel.Current.MessageBox(MessageBoxType.Info, "OK");
         //   AuditTrailViewModel.Secondary.LoadAuditTrail();
         //};
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private DataSet DataTable2DataSet(DataTable dataTable)
      {
         DataSet Result = new DataSet();
         Result.Tables.Add(dataTable);
         return Result;
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    

      private void btnExcel_Click(object sender, RoutedEventArgs e)
      {
#if REPORTING
         // Configure open file dialog box
         Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

         dlg.Title = "Report";
         dlg.FileName = ReportingViewModel.Instance.GetFileName(ReportingViewModel.ReportTypes.XLSX);
         dlg.Filter = "Excel document (.xlsx)|*.xlsx|Zip Files (.zip)|*.zip";
         dlg.DefaultExt = ".xlsx";
         dlg.InitialDirectory = ReportingViewModel.Instance.InitialDirectory;

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog(Application.Current.MainWindow);

         // Process open file dialog box results 
         if (result == true)
         {
            if (File.Exists(dlg.FileName))
            {
               File.Delete(dlg.FileName);
            };

            ReportingViewModel.Instance.PrepareData(DoXLS, dlg.FileName);
         };
      }

      void DoXLS(string Msg, string FileName)
      {
         if (ReportEngine.Instance.ReportSource.Tables.Count == 0 || ReportEngine.Instance.ReportSource.Tables[0].Rows.Count == 0)
            throw new Exception("No data available");

         // AuditTrail.Write("REPORT", "DoExcel:" + ReportingViewModel.Instance.SelectedReport.Title);

         ReportEngine report = ReportEngine.Instance;

         BackboneViewModel.Current.IncBusy();
         report.SaveExcelReport(FileName);
         BackboneViewModel.Current.DecBusy();

         //if (ReportingViewModel.Instance.AutoLaunch && File.Exists(dlg.FileName))
         if (File.Exists(FileName))
         {
                     //new Process
         {
            StartInfo = new ProcessStartInfo(FileName)
            {
               UseShellExecute = true
            }
         }.Start();

         };
#endif
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    

      private void btnPDF_Click(object sender, RoutedEventArgs e)
      {
#if REPORTING
         // Configure open file dialog box
         Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

         dlg.Title = "Report";
         dlg.FileName = ReportingViewModel.Instance.GetFileName(ReportingViewModel.ReportTypes.PDF);
         dlg.Filter = "Acrobat document (.pdf)|*.pdf";
         dlg.DefaultExt = ".pdf";

         dlg.InitialDirectory = ReportingViewModel.Instance.InitialDirectory;

         // Show open file dialog box
         Nullable<bool> result = dlg.ShowDialog(Application.Current.MainWindow);

         // Process open file dialog box results 
         if (result == true)
         {
            if (File.Exists(dlg.FileName))
            {
               File.Delete(dlg.FileName);
            };

            ReportingViewModel.Instance.PrepareData(DoPDF, dlg.FileName);
         };
      }

      void DoPDF(string Msg, string FileName)
      {
         if (ReportEngine.Instance.ReportSource.Tables.Count == 0 || ReportEngine.Instance.ReportSource.Tables[0].Rows.Count == 0)
            throw new Exception("No data available");

         // AuditTrail.Write("REPORT", "DoPDF:" + ReportingViewModel.Instance.SelectedReport.Title);

         ReportEngine report = ReportEngine.Instance;

         BackboneViewModel.Current.IncBusy();
         bool Result = report.SavePDFReport(FileName);
         BackboneViewModel.Current.DecBusy();

         // if (ReportingViewModel.Instance.AutoLaunch && File.Exists(dlg.FileName))
         if (Result && File.Exists(FileName))
         {
         new Process
         {
            StartInfo = new ProcessStartInfo(FileName)
            {
               UseShellExecute = true
            }
         }.Start();

         };
#endif
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    

      private void Page_Loaded(object sender, RoutedEventArgs e)
      {
      }

      private void Page_Unloaded(object sender, RoutedEventArgs e)
      {
         cbAutoRefresh.IsChecked = false;

         cbAutoRefresh.CheckBox.Checked -= CheckBox_Checked;
         cbAutoRefresh.CheckBox.Unchecked -= CheckBox_Checked;

         //ToDo: if (MainViewModel.Current.AutoOpenClose)
         //{
         //   DBViewModel.Current.Connection.Close();
         //   DB_SQL._ViewModel.Close();
         //};
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    

      /// <summary>
      /// Removes all event handlers subscribed to the specified routed event from the specified element.
      /// </summary>
      /// <param name="element">The UI element on which the routed event is defined.</param>
      /// <param name="routedEvent">The routed event for which to remove the event handlers.</param>
      public static void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
      {
         // Get the EventHandlersStore instance which holds event handlers for the specified element.
         // The EventHandlersStore class is declared as internal.
         PropertyInfo eventHandlersStoreProperty = typeof(UIElement).GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
         object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

         // If no event handlers are subscribed, eventHandlersStore will be null.
         // Credit: https://stackoverflow.com/a/16392387/1149773
         if (eventHandlersStore == null)
         {
            return;
         }

         // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
         // for getting an array of the subscribed event handlers.
         MethodInfo getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         RoutedEventHandlerInfo[] routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(eventHandlersStore, new object[] { routedEvent });

         // Iteratively remove all routed event handlers from the element.
         foreach (RoutedEventHandlerInfo routedEventHandler in routedEventHandlers)
         {
            element.RemoveHandler(routedEvent, routedEventHandler.Handler);
         }
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    

      private void btnGo_Click(object sender, RoutedEventArgs e)
      {
         _AuditTrailViewModel.Level = (ErrorLevel)(cbFilterLevel.ComboBox.SelectedItem);
         _AuditTrailViewModel.TerminalID = (string)cbFilterTerminal.ComboBox.SelectedValue;
         _AuditTrailViewModel.FKUser = (string)cbFilterUser.ComboBox.SelectedValue;
         _AuditTrailViewModel.Childs = cbFilterChilds.IsChecked == true;
         _AuditTrailViewModel.IsDebug = cbFilterDebug.IsChecked == true;

         //_AuditTrailViewModel.Event = DropDownListEvent.SelectedValue;

         BackboneViewModel.Current.IncBusy();

         DoIt.OnBackground(() =>
         {
            _AuditTrailViewModel.LoadAuditTrail();
            BackboneViewModel.Current.DecBusy();
         });
      }

      private void btnClear_Click(object sender, RoutedEventArgs e)
      {
         _AuditTrailViewModel.Level = ErrorLevel.Info;
         cbFilterLevel.ComboBox.SelectedItem = _AuditTrailViewModel.Level;

         _AuditTrailViewModel.TerminalID = "";
         cbFilterTerminal.ComboBox.SelectedItem = _AuditTrailViewModel.TerminalID;

         _AuditTrailViewModel.FKUser = "";
         cbFilterUser.ComboBox.SelectedItem = _AuditTrailViewModel.FKUser;

         _AuditTrailViewModel.Childs = false;
         cbFilterChilds.IsChecked = _AuditTrailViewModel.Childs;

         _AuditTrailViewModel.IsDebug = false;
         cbFilterDebug.IsChecked = _AuditTrailViewModel.IsDebug;
      }

      private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
      {

      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    
   }
}
