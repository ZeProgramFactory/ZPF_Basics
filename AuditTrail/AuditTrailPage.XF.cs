using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using ZPF.XF;
//using ZPF.SQL;
using ZPF.AT;
using ZPF;
using System.IO;
using ZPF.XF.Compos;

public class AuditTrailPage : Page_Base
{
   public AuditTrailPage()
   {
      Title = "logs";

      var g = new Grid()
      {
         Margin = new Thickness(10, 0, 10, 10),
      };

      g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
      g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

      g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
      g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
      g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
      g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

      ListView listView = null;

      {
         listView = AuditTrailListView.Get(this);
         listView.Margin = new Thickness(2, 0, 2, 0);

         g.Children.Add(listView, 0, 4, 1, 2);

         listView.BindingContext = AuditTrailViewModel.Current;
         listView.SetBinding(ListView.ItemsSourceProperty, "Logs");
      };

      // - - -  - - - 

      SetMainContent(g);
      SetAppBarContent();

      // - - -  - - - 

      string AuditTrailFileName = ZPF.XF.Basics.Current.FileIO.GetDataDirectory() + @"AuditTrail.txt";
      System.Diagnostics.Debug.WriteLine(AuditTrailFileName);

      AuditTrailViewModel.Current.Init(new FileAuditTrailWriter(AuditTrailFileName));

      // - - -  - - - 

      BackboneViewModel.Current.IncBusy();

      // - - -  - - - 

      I18nViewModel.Current.T(this);
   }

   protected override void OnAppearing()
   {
      base.OnAppearing();

      // - - -  - - - 

      DoIt.OnMainThread(() =>
      {
         BackboneViewModel.Current.DecBusy();
      });
   }

   protected override void OnDisappearing()
   {
      base.OnDisappearing();
   }
}
