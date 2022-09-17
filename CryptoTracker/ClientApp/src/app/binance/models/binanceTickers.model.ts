export interface BinanceTickersModel{
    tickers?:BinancePriceTickerModel[];
}

export interface BinancePriceTickerModel{
    symbol?: string;
    price?: number;
}