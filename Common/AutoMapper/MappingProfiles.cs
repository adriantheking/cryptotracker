using AutoMapper;
using CryptoDatabase.Repositories.Binance;
using CryptoDatabase.Repositories.Zonda;
using BinanceModel = Models.Connectors.Binance;
using ZondaModel = Models.Connectors.Zonda;

namespace CryptoCommon.AutoMapper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<BinanceModel.BinanceC2CTradeHistoryData, BinanceC2CTradeHistoryData>();
            CreateMap<ZondaModel.ZondaOperationHistoryModel, ZondaOperationHistory>();
            CreateMap<ZondaModel.ZondaOperationHistoryItemModel, ZondaOperationHistoryItem>();
            CreateMap<ZondaModel.ZondaOperationHistoryBalanceModel, ZondaOperationHistoryBalance>();
            CreateMap<ZondaModel.ZondaOperationHistoryChangeModel, ZondaOperationHistoryChange>();
            CreateMap<ZondaModel.ZondaOperationHistoryFundsAfterModel, ZondaOperationHistoryFundsAfter>();
            CreateMap<ZondaModel.ZondaOperationHistoryFundsBeforeModel, ZondaOperationHistoryFundsBefore>();
            CreateMap<ZondaModel.ZondaOperationHistorySettingModel, ZondaOperationHistorySetting>();
        }
    }
}
