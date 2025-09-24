using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace CoinApiApp.Models
{
    public class CryptoCurrency
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("current_price")]
        public decimal CurrentPrice { get; set; }

        [JsonPropertyName("market_cap")]
        public decimal MarketCap { get; set; }

        [JsonPropertyName("price_change_percentage_24h")]
        public decimal PriceChangePercentage24h { get; set; }

        // Нові поля
        [JsonPropertyName("total_supply")]
        public decimal TotalSupply { get; set; }            // Макс. кількість монет
        [JsonPropertyName("circulating_supply")]
        public decimal CirculatingSupply { get; set; }      // В обігу
        [JsonPropertyName("total_volume")]
        public decimal TotalVolume { get; set; }
    }
}
