using CoinApiApp.Service;
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

namespace CoinApiApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var api = new ApiService();
            var currencies = await api.GetTopCurrenciesAsync();

            foreach (var c in currencies)
            {
                Console.WriteLine($"{c.Name} ({c.Symbol}): {c.CurrentPrice} USD, change: {c.PriceChangePercentage24h}%");
            }
        }
    }
}
