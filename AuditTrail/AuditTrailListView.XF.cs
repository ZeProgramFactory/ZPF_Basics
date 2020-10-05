using Xamarin.Forms;
using ZPF.Conv;

namespace ZPF.XF.Compos
{
   public static class AuditTrailListView
   {
      public static ListView Get(Page owner)
      {
         return new ListView(ListViewCachingStrategy.RecycleElement)
         {
            BackgroundColor = Xamarin.Forms.Color.FromHex("2FFF"),
            //HasUnevenRows = true,
            Margin = 5,
            //RowHeight = 76,
            //IsPullToRefreshEnabled = true,

            ItemTemplate = new DataTemplate(() =>
            {
               var g = GetGrid();

            // - - -  - - -

            // Return an assembled ViewCell.
            var v = new ViewCell
               {
                  View = g
               };

               return v;
            }),
         };
      }

      public static Grid GetGrid(bool AllFields = true)
      {
         var gh = new Grid()
         {
            Margin = new Thickness(5, 5, 5, 0),
            BackgroundColor = Xamarin.Forms.Color.FromHex("6FFF"),
         };

         gh.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(52, GridUnitType.Absolute) });

         {
            var gv = new Grid
            {
               WidthRequest = 52,
               HeightRequest = 52,
               VerticalOptions = LayoutOptions.Start,
               HorizontalOptions = LayoutOptions.Center,
               BackgroundColor = Xamarin.Forms.Color.Beige,
               Margin = new Thickness(0, 0, 0, 5),
            };
            gv.SetBinding(Grid.BackgroundColorProperty, new Binding("Background", BindingMode.OneWay, new ColorConverter()));

            var sp = new StackLayout
            {
               VerticalOptions = LayoutOptions.Center,
               HorizontalOptions = LayoutOptions.Center,
            };

            double fontSize = 11;

            switch (Device.RuntimePlatform)
            {
               case Device.macOS:
                  fontSize = 9.5;
                  break;

               case Device.UWP:
                  fontSize = 11;
                  break;
            };

            {
               var l = new Label
               {
                  HorizontalOptions = LayoutOptions.Center,
                  FontAttributes = FontAttributes.Bold,
                  FontSize = fontSize
               };
               l.SetBinding(Label.TextProperty, new Binding("TimeStamp", BindingMode.OneWay, new DateTimeToStringConverter(), "MM/dd", null));
               l.SetBinding(Label.TextColorProperty, new Binding("Foreground", BindingMode.OneWay, new ColorConverter()));
               sp.Children.Add(l);
            };

            {
               var l = new Label
               {
                  HorizontalOptions = LayoutOptions.Center,
                  FontAttributes = FontAttributes.Bold,
                  FontSize = fontSize
               };
               l.SetBinding(Label.TextProperty, new Binding("TimeStamp", BindingMode.OneWay, new DateTimeToStringConverter(), "HH:mm:ss", null));
               l.SetBinding(Label.TextColorProperty, new Binding("Foreground", BindingMode.OneWay, new ColorConverter()));
               sp.Children.Add(l);
            };

            gv.Children.Add(sp, 0, 0);
            gh.Children.Add(gv, 0, 0);
         };

         {
            var l = new Label
            {
               VerticalOptions = LayoutOptions.StartAndExpand,
            };
            l.SetBinding(Label.TextProperty, "Message");

            gh.Children.Add(l, 1, 0);
         };

         return gh;
      }
   }
}

