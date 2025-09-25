using CoinApiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoinApiApp.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async Task<List<CryptoCurrency>> GetTopCurrenciesAsync(int count = 10)
        {
            string url = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var currencies = JsonSerializer.Deserialize<List<CryptoCurrency>>(json, options);

            return currencies ?? new List<CryptoCurrency>();
        }
        public async Task<List<(DateTime Date, decimal Price)>> GetCoinHistoryAsync(string coinId, int days = 7)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=usd&days={days}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using (var doc = JsonDocument.Parse(json))
                {
                    var prices = doc.RootElement.GetProperty("prices");
                    var result = new List<(DateTime, decimal)>();

                    foreach (var item in prices.EnumerateArray())
                    {
                        // item[0] = timestamp (мс)
                        // item[1] = ціна
                        long timestamp = item[0].GetInt64();
                        decimal price = item[1].GetDecimal();

                        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                        result.Add((date, price));
                    }

                    return result;
                }
            }
        }

    }
}
