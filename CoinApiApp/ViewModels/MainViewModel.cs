using CoinApiApp.Models;
using CoinApiApp.Service;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoinApiApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //public ObservableCollection<CryptoCurrency> Currencies { get; set; }
        private ObservableCollection<CryptoCurrency> _allCurrencies = new ObservableCollection<CryptoCurrency>();
        public ObservableCollection<CryptoCurrency> Currencies { get; set; } = new ObservableCollection<CryptoCurrency>();

        private readonly ApiService _apiService;

        private CryptoCurrency _selectedCoin;
        public CryptoCurrency SelectedCoin
        {
            get => _selectedCoin;
            set
            {
                _selectedCoin = value;
                OnPropertyChanged();

                if (_selectedCoin != null)
                    ShowDetailsCommand.Execute(_selectedCoin);
            }
        }

        public ICommand ShowDetailsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SortCommand { get; }
        public MainViewModel()
        {
            Currencies = new ObservableCollection<CryptoCurrency>();
            _apiService = new ApiService();

            ShowDetailsCommand = new RelayCommand<CryptoCurrency>(async coin =>
            {
                //var window = new Views.CoinDetails
                //{
                //    DataContext = new DetailsViewModel(coin)
                //};
                //window.ShowDialog();

                var history = await _apiService.GetCoinHistoryAsync(coin.Id);

                var prices = history.Select(h => h.Price).ToList();
                var dates = history.Select(h => h.Date.ToString("dd.MM")).ToList();

                var window = new Views.CoinDetails
                {
                    DataContext = new DetailsViewModel(coin, prices, dates)
                };
                window.ShowDialog();
            });

            SearchCommand = new RelayCommand<object>(_ => ApplySearchFilter());
            RefreshCommand = new RelayCommand<object>(_ => LoadCurrencies());
            SortCommand = new RelayCommand<string>(criteria => ApplySort(criteria));

            LoadCurrencies();
        }
        private bool orderName = true;
        private bool orderMarket = true;
        private void ApplySort(string criteria)
        {
            IEnumerable<CryptoCurrency> sorted;

            switch (criteria)
            {
                case "Name":
                    if (orderName)
                    {
                        sorted = Currencies.OrderBy(c => c.Name).ToList();
                        orderName = false;
                    }
                    else
                    {
                        sorted = Currencies.OrderByDescending(c => c.Name).ToList();
                        orderName = true;
                    }
                    break;
                case "MarketCap":
                    if (orderMarket)
                    {
                        sorted = Currencies.OrderBy(c => c.MarketCap).ToList();
                        orderMarket = false;
                    }
                    else
                    {
                        sorted = Currencies.OrderByDescending(c => c.MarketCap).ToList();
                        orderMarket = true;
                    }
                    break;
                default:
                    sorted = Currencies.ToList();
                    break;
            }

            // Перезаписуємо колекцію
            //Currencies = new ObservableCollection<CryptoCurrency>(sorted);
            //OnPropertyChanged(nameof(Currencies));
            Currencies.Clear();
            foreach (var currency in sorted)
                Currencies.Add(currency);
        }

        private int _coinCount = 10;
        public int CoinCount
        {
            get => _coinCount;
            set
            {
                if (_coinCount != value)
                {
                    _coinCount = value;
                    OnPropertyChanged();
                    LoadCurrencies(); // перезавантажуємо список при зміні
                }
            }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }


        private void ApplySearchFilter()
        {
            Currencies.Clear();

            IEnumerable<CryptoCurrency> filtered;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = _allCurrencies;
            }
            else
            {
                filtered = _allCurrencies
                    .Where(c => c.Name.Contains(SearchText)
                             || c.Symbol.Contains(SearchText));
            }

            foreach (var c in filtered)
                Currencies.Add(c);
        }
        private async void LoadCurrencies()
        {
            try
            {
                var list = await _apiService.GetTopCurrenciesAsync(CoinCount);

                if (list != null)
                {
                    _allCurrencies.Clear();
                    foreach (var c in list)
                        _allCurrencies.Add(c);

                    ApplySearchFilter(); // одразу застосовуємо пошук/фільтр
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error loading data");
            }
        }
    }
}
