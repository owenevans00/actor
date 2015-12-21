using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfActor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel.ViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            this._viewModel = new ViewModel.ViewModel(this.Dispatcher);
            this.DataContext = this._viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // this._viewModel.ResponseTimes.Add(tbUrl.Text);
            _viewModel.GetUrlResponseTime(tbUrl.Text);
        }


    }
}
