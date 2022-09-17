import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IWallet } from 'src/app/models/wallet.model';
import { environment } from 'src/environments/environment';
import { BinanceTickersModel } from '../models/binanceTickers.model';

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
  public GetWallet(): Observable<IWallet> {
    return this.httpClient.get(environment.API_URL + '/Dashboard/GetWallet') as Observable<IWallet>;
  }
}
