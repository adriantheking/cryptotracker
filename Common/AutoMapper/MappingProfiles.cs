using AutoMapper;
using CryptoDatabase.Repositories.Binance;
using BinanceModel = Models.Connectors.Binance;

namespace CryptoCommon.AutoMapper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<BinanceModel.BinanceC2CTradeHistoryData, BinanceC2CTradeHistoryData>();
        }
    }
}
