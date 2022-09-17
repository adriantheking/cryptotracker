import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import 'rxjs/add/observable/of';
import { map, share } from 'rxjs/operators';
import { IWallet } from 'src/app/models/wallet.model';
import { environment } from 'src/environments/environment';
import { BinanceTickersModel } from '../models/binanceTickers.model';
import { Share } from '@ngspot/rxjs/decorators';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {

  constructor(private httpClient: HttpClient) {

  }

  public GetInvestedAmount(): Observable<number> {
    return this.httpClient.get(environment.API_URL + "/Binance/GetInvestedAmount") as Observable<number>;
  }

  public GetTickers(): Observable<BinanceTickersModel> {
    return this.httpClient.get(environment.API_URL + '/binance/GetTickers') as Observable<BinanceTickersModel>;
  }

  @Share()
  public GetWallet(): Observable<IWallet> {
    return this.httpClient.get(environment.API_URL + '/Dashboard/GetWallet');
  }



  public GetTotalCoinsWorth():Promise<number> {
    return new Promise(resolver => {
      this.GetWallet().subscribe(wallet => {
        let sum = 0;
        if (wallet.coins != undefined) {
          for (var i = 0; i < wallet.coins.length; i++) {
            if (wallet.coins[i].totalInvested != undefined) {
              sum += wallet.coins[i].totalInvested!;
            }
          }
        }
        resolver(sum);
      })
    });
  }
}
