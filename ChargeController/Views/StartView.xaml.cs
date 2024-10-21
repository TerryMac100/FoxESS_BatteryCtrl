using ChargeController.ViewModels;
using System.Windows.Controls;

namespace ChargeController.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartView : UserControl
    {
        public StartView(StartViewModel startViewModel)
        {
            InitializeComponent();
            DataContext = startViewModel;
        }
    }
}
