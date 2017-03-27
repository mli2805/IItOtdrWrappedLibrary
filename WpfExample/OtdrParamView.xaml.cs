using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for OtdrParamView.xaml
    /// </summary>
    public partial class OtdrParamView
    {
        public OtdrParamView()
        {
            InitializeComponent();
        }

        private void RbCount_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton) sender;
            if (rb.Name == "RbCount")
            {
                RbCount.Foreground = Brushes.Black;
                RbTime.Foreground = Brushes.DarkGray;
                CbCounts.Foreground = Brushes.Black;
                CbTimes.Foreground = Brushes.DarkGray;
            }
        }
    }
}
