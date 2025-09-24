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
        
    }
}
