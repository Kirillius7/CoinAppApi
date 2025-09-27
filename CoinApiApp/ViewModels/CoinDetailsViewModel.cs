
using CoinApiApp.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace CoinApiApp.ViewModels
{
    public class DetailsViewModel
    {
        public CryptoCurrency Coin { get; set; }
        public PlotModel PricePlotModel { get; set; }

        public DetailsViewModel(CryptoCurrency coin, List<decimal> prices, List<string> dates)
        {
            Coin = coin;

            var parsedDates = dates.Select(d => DateTime.Parse(d)).ToList();

            // Групування по днях і обчислення середнього значення ціни
            var grouped = parsedDates
                .Zip(prices, (date, price) => new { date, price })
                .GroupBy(x => x.date.Date)
                .Select(g => new
                {
                    Day = g.Key,
                    AvgPrice = g.Average(x => (double)x.price)
                })
                .OrderBy(g => g.Day)
                .ToList();

            // Створення нових списків з усередненими значеннями
            var datesPerDay = grouped.Select(g => g.Day.ToString("dd.MM")).ToList();
            var pricesPerDay = grouped.Select(g => g.AvgPrice).ToList();

            // Побудова графіку OxyPlot
            PricePlotModel = new PlotModel { Title = $"{coin.Name} - Price History (Daily Avg)" };

            var lineSeries = new LineSeries
            {
                Title = coin.Name,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.Blue
            };

            for (int i = 0; i < pricesPerDay.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, pricesPerDay[i]));
            }

            PricePlotModel.Series.Add(lineSeries);

            // X-вісь (дні)
            PricePlotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Angle = 45,
                ItemsSource = datesPerDay
            });

            // Y-вісь (ціни)
            PricePlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Price",
                StringFormat = "F4"
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name => Coin.Name;
        public string Symbol => Coin.Symbol;
        public decimal CurrentPrice => Coin.CurrentPrice;
        public decimal MarketCap => Coin.MarketCap;
        public decimal PriceChangePercentage24h => Coin.PriceChangePercentage24h;
        public decimal TotalSupply => Coin.TotalSupply;
        public decimal CirculatingSupply => Coin.CirculatingSupply;
        public decimal TotalVolume => Coin.TotalVolume;
    }

}
