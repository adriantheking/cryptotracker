import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import 'rxjs/add/observable/of';
import { map, share } from 'rxjs/operators';
import { IWallet } from 'src/app/models/wallet.model';
import { environment } from 'src/environments/environment';
import { BinanceTickersModel } from '../models/binanceTickers.model';
import { Share } from '@ngspot/rxjs/decorators';
import { BinanceCoinsValueModel } from '../models/binanceCoinsValue.model';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {

  constructor(private httpClient: HttpClient) {

  }

  public GetInvestedAmount(): Observable<number> {
    return this.httpClient.get(environment.API_URL + "/Binance/GetInvestedAmount") as Observable<number>;
  }
  @Share()
  public GetTickers(): Observable<BinanceTickersModel[]> {
    return this.httpClient.get(environment.API_URL + '/binance/GetTickers') as Observable<BinanceTickersModel[]>;
  }

  @Share()
  public GetWallet(): Observable<IWallet> {
    return this.httpClient.get(environment.API_URL + '/Dashboard/GetWallet');
  }



  public CalculateCoinsWorth(): Promise<BinanceCoinsValueModel[]> {
    return new Promise(resolver => {
      var output: BinanceCoinsValueModel[] = [];

      this.GetWallet().subscribe(wallet => {
        this.GetTickers().subscribe(tickerData => {
          if (wallet.coins != undefined) {
            for (var i = 0; i < wallet.coins.length; i++) {
              if (wallet.coins[i].symbol != undefined) {
                var coinRecord: BinanceCoinsValueModel = {};
                var ticker = tickerData[0].tickers?.find(x => x.symbol?.toLocaleLowerCase() == wallet.coins![i].symbol?.toLocaleLowerCase());
                coinRecord.symbol = wallet.coins[i].symbol;
                coinRecord.value = (ticker?.price! * wallet.coins[i].amount!);
                output.push(coinRecord);
              }
            }
          }
          resolver(output);
        })
      })
    });
  }
}
