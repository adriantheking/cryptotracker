using Newtonsoft.Json;

namespace Models.Connectors.Zonda
{
    public class ZondaMarketStatsModel
    {
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("stats")]
        public ZondaMarketStatsDetailsModel? Stats { get; set; }
    }

    /// <summary>
    /// Details for parameters: https://docs.zonda.exchange/reference/statystyki-rynku-1
    /// </summary>
    public class ZondaMarketStatsDetailsModel
    {
        /// <summary>
        /// Market code
        /// </summary>
        [JsonProperty("m")]
        public string? M { get; set; }
        /// <summary>
        /// Highest 24h value
        /// </summary>
        [JsonProperty("h")] 
        public decimal? H { get; set; }
        /// <summary>
        /// Lowest 24h value
        /// </summary>
        [JsonProperty("l")] 
        public decimal? L { get; set; }
        /// <summary>
        /// Volumen
        /// </summary>
        [JsonProperty("v")] 
        public decimal? V { get; set; }
        /// <summary>
        /// Open rate
        /// </summary>
        [JsonProperty("r24h")] 
        public decimal? R24h { get; set; }
    }
}
