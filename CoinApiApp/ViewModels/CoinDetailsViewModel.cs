
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
        // Серії для графіка
        public PlotModel PricePlotModel { get; set; }

        public DetailsViewModel(CryptoCurrency coin, List<decimal> prices, List<string> dates)
        {
            Coin = coin;

            //PricePlotModel = new PlotModel { Title = $"{coin.Name} - Price History" };

            //// Додаємо лінійну серію
            //var lineSeries = new LineSeries
            //{
            //    Title = coin.Name,
            //    MarkerType = MarkerType.None
            //};

            //// Додаємо дані
            //for (int i = 0; i < prices.Count; i++)
            //{
            //    // Для X-ось можна використовувати індекс, а підписи будемо задавати через LabelFormatter
            //    lineSeries.Points.Add(new DataPoint(i, (double)prices[i]));
            //}

            //PricePlotModel.Series.Add(lineSeries);

            //// Ось X з підписами дат
            //PricePlotModel.Axes.Add(new CategoryAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    Angle = 45,
            //    ItemsSource = dates // сюди передаємо список дат
            //});

            //// Ось Y для цін
            //PricePlotModel.Axes.Add(new LinearAxis
            //{
            //    Position = AxisPosition.Left,
            //    Title = "Price",
            //    StringFormat = "C"
            //});

            var parsedDates = dates.Select(d => DateTime.Parse(d)).ToList();

            // 2️⃣ Групуємо по днях і обчислюємо середнє значення ціни
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

            // 3️⃣ Створюємо нові списки з усередненими значеннями
            var datesPerDay = grouped.Select(g => g.Day.ToString("dd.MM")).ToList();
            var pricesPerDay = grouped.Select(g => g.AvgPrice).ToList();

            // 4️⃣ Побудова графіку
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
        public decimal MarketCap => Coin.MarketCap; // decimal
        public decimal PriceChangePercentage24h => Coin.PriceChangePercentage24h;
        public decimal TotalSupply => Coin.TotalSupply;
        public decimal CirculatingSupply => Coin.CirculatingSupply;
        public decimal TotalVolume => Coin.TotalVolume;
    }

}
