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
            _httpClient = new HttpClient(); // ініціалізація поля для роботи із запитами для подальшого введення даних
            _httpClient.BaseAddress = new Uri("https://api.coingecko.com/api/v3/"); // базова адреса
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async Task<List<CryptoCurrency>> GetTopCurrenciesAsync(int count = 10)
        {
            // створення запиту типу GET з інтерполяцією (кількістю монет для виведення даних)
            string url = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
            var response = await _httpClient.GetAsync(url);

            // перевірка отриманого коду відповіді
            response.EnsureSuccessStatusCode();

            // асинхронне зчитування відповіді 
            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions // налаштування системи Json 
            { 
                PropertyNameCaseInsensitive = true
            };

            // виведення (десеріалізація) рядків у список об'єктів CryptoCurrency
            var currencies = JsonSerializer.Deserialize<List<CryptoCurrency>>(json, options);

            return currencies ?? new List<CryptoCurrency>();
        }

        // повернення кортежу - дні та ціни 
        public async Task<List<(DateTime Date, decimal Price)>> GetCoinHistoryAsync(string coinId, int days = 7)
        {
            using (var client = new HttpClient())
            {
                // формування запиту на пошук історії монети та протягом якого часу
                string url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=usd&days={days}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // парсинг JSON у об’єкт JsonDocument
                using (var doc = JsonDocument.Parse(json))
                {
                    // збір "prices" з JSON (де розташований час та вартість)
                    var prices = doc.RootElement.GetProperty("prices");
                    // створення кортежа
                    var result = new List<(DateTime, decimal)>();

                    foreach (var item in prices.EnumerateArray())
                    {
                        // робота з кожним елементом масиву prices
                        long timestamp = item[0].GetInt64();
                        decimal price = item[1].GetDecimal();

                        // конвертація Unix timestamp в DateTime
                        var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                        result.Add((date, price));
                    }

                    return result;
                }
            }
        }

    }
}
