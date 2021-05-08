using System.Windows.Controls;
using ZPF.AT;

namespace ZPF
{
   /// <summary>
   /// Interaction logic for AuditTrail_Details_Page.xaml
   /// </summary>
   public partial class AuditTrail_Details_Page : Page
   {
      public AuditTrail_Details_Page(AuditTrailViewModel _AuditTrailViewModel)
      {
         DataContext = _AuditTrailViewModel;

         InitializeComponent();
      }
   }
}
