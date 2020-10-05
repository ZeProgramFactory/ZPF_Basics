using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ZPF.WPF
{
   public class AuditTrailData : Grid
   {
      TextBox _TextBox = null;
      ScrollViewer _ScrollViewer = null;
      TextBlockEx _TextBlock = null;
      WebBrowser _WebBrowser = null;

      public AuditTrailData()
      {
         this.Background = new SolidColorBrush(ColorHelper.FromHex("#8FFF"));

         // - - -  - - - 
         _TextBox = new TextBox();
         _TextBox.TextWrapping = TextWrapping.Wrap;
         _TextBox.IsReadOnly = true;
         _TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
         _TextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

         this.Children.Add(_TextBox);

         // - - -  - - - 

         _ScrollViewer = new ScrollViewer();
         _TextBlock = new TextBlockEx();
         _TextBlock.TextWrapping = TextWrapping.Wrap;
         _ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
         _ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
         _ScrollViewer.Content = _TextBlock;
         this.Children.Add(_ScrollViewer);

         // - - -  - - - 

         _WebBrowser = new WebBrowser();
         _WebBrowser.OpacityMask = new SolidColorBrush(ColorHelper.FromHex("#8FFF"));

         this.Children.Add(_WebBrowser);

         // - - -  - - - 

         Redraw();

         this.SizeChanged += AuditTrailData_SizeChanged;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      private void AuditTrailData_SizeChanged(object sender, SizeChangedEventArgs e)
      {
         Redraw();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      static FrameworkPropertyMetadata propertyMetadata_DataType = new FrameworkPropertyMetadata(
            "",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            new PropertyChangedCallback(DataType_PropertyChanged),
            new CoerceValueCallback(DataType_CoerceDataType),
            false,
            UpdateSourceTrigger.PropertyChanged
         );

      public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(AuditTrailData), propertyMetadata_DataType, new ValidateValueCallback(DataType_Validate));

      private static void DataType_PropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
      {
         //To be called whenever the DP is changed.
         //Debug.WriteLine(string.Format("Property changed is fired : OldDataType {0} NewDataType : {1}", e.OldValue, e.NewValue));

         string st = e.NewValue as string;

         (dobj as AuditTrailData)._DataType = st;
         (dobj as AuditTrailData).Redraw();

      }

      private static object DataType_CoerceDataType(DependencyObject dobj, object value)
      {
         //called whenever dependency property value is reevaluated. The return value is the
         //latest value set to the dependency property
         //Debug.WriteLine(string.Format("CoerceDataType is fired : DataType {0}", value));

         return value;
      }

      private static bool DataType_Validate(object value)
      {
         //Custom validation block which takes in the value of DP
         //Returns true / false based on success / failure of the validation
         //Debug.WriteLine(string.Format("DataValidation is Fired : DataType {0}", value));

         return true;
      }

      public string DataType
      {
         get
         {
            return this.GetValue(DataTypeProperty) as string;
         }
         set
         {
            this.SetValue(DataTypeProperty, value);
            Redraw();
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      static FrameworkPropertyMetadata propertyMetadata = new FrameworkPropertyMetadata(
            "",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            new PropertyChangedCallback(Data_PropertyChanged),
            new CoerceValueCallback(Data_CoerceData),
            false,
            UpdateSourceTrigger.PropertyChanged
         );

      public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(string), typeof(AuditTrailData), propertyMetadata, new ValidateValueCallback(Data_Validate));

      private static void Data_PropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
      {
         //To be called whenever the DP is changed.
         //Debug.WriteLine(string.Format("Property changed is fired : OldData {0} NewData : {1}", e.OldValue, e.NewValue));

         string st = e.NewValue as string;

         (dobj as AuditTrailData)._Data = st;
         (dobj as AuditTrailData).Redraw();
      }

      private static object Data_CoerceData(DependencyObject dobj, object value)
      {
         //called whenever dependency property value is reevaluated. The return value is the
         //latest value set to the dependency property
         //Debug.WriteLine(string.Format("CoerceData is fired : Data {0}", value));

         return value;
      }

      private static bool Data_Validate(object value)
      {
         //Custom validation block which takes in the value of DP
         //Returns true / false based on success / failure of the validation
         //Debug.WriteLine(string.Format("DataValidation is Fired : Data {0}", value));

         return true;
      }

      public string Data
      {
         get
         {
            return this.GetValue(DataProperty) as string;
         }
         set
         {
            this.SetValue(DataProperty, value);
            Redraw();
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      private string _Data = "";
      private string _DataType = "";

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

      private void Redraw()
      {
         string dt = null;

         if (_DataType != null)
         {
            dt = _DataType.Trim().ToUpper();
         };

         if (string.IsNullOrEmpty(_Data))
         {
            dt = "";
         };

         switch (dt)
         {
            case "SQL":
               _TextBox.Visibility = Visibility.Visible;
               _ScrollViewer.Visibility = Visibility.Collapsed;
               _WebBrowser.Visibility = Visibility.Collapsed;
               _TextBlock.Width = (this.Width > 20 ? this.Width - 20 : 0);
               //_TextBox.Text = SQLHelper.FormatSQL(_Data);
               _TextBox.Text = _Data;
               break;

            case "TXTEX":
               _TextBox.Visibility = Visibility.Collapsed;
               _ScrollViewer.Visibility = Visibility.Visible;
               _WebBrowser.Visibility = Visibility.Collapsed;
               _TextBlock.Width = (this.Width > 20 ? this.Width - 20 : 0);
               _TextBlock.TextEx = _Data;
               break;

            case "HTML":
               _WebBrowser.Visibility = Visibility.Visible;
               _ScrollViewer.Visibility = Visibility.Collapsed;
               _TextBox.Visibility = Visibility.Collapsed;

               _WebBrowser.NavigateToString(_Data);
               break;

            default:
               _TextBox.Visibility = Visibility.Visible;
               _TextBlock.Width = (this.Width>20 ?  this.Width - 20 : 0 );
               _ScrollViewer.Visibility = Visibility.Collapsed;
               _WebBrowser.Visibility = Visibility.Collapsed;
               _TextBox.Text = _Data;
               break;
         };

         //if (_Image.Source == null)
         //{
         //   _Image.Width = 0;
         //   _Image.Height = 0;

         //   _TextBlock.VerticalAlignment = VerticalAlignment.Center;
         //};

         //if (Width > 50)
         //{
         //   _TextBlock.Visibility = Visibility.Visible;
         //   _StackPanel.Margin = new Thickness(8);

         //   if (_Image.Source != null)
         //   {
         //      _Image.Width = 60;
         //      _Image.Height = 60;
         //   };

         //   this.ToolTip = null;
         //}
         //else
         //{
         //   _TextBlock.Visibility = Visibility.Collapsed;
         //   _StackPanel.Margin = new Thickness(0);
         //   _StackPanel.VerticalAlignment = VerticalAlignment.Center;
         //   _Grid.Margin = _ImageMarginSmall;

         //   if (_Image.Source != null)
         //   {
         //      _Image.Width = Width - _ImageMarginSmall.Left - _ImageMarginSmall.Right;
         //      _Image.Height = Width - _ImageMarginSmall.Top - _ImageMarginSmall.Bottom;
         //   };

         //   this.ToolTip = _TextBlock.Text;
         //};
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  
   }
}
