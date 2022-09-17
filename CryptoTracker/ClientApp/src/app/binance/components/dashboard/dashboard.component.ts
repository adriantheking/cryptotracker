import { Component, OnInit } from '@angular/core';
import { IWallet } from 'src/app/models/wallet.model';
import { BinanceTickersModel } from '../../models/binanceTickers.model';
import { BinanceService } from '../../services/binance.service';

@Component({
  selector: 'binance-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public wallet: IWallet = {};
  public tickers: BinanceTickersModel[] = [];
  
  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.getWallet();
    this.getTickers();
    this.binanceService.CalculateCoinsWorth().then( x => console.log(x));
  }

  private getWallet(){
    this.binanceService.GetWallet().subscribe(x => {
      this.wallet = x
    });
  }

  private getTickers(){
    this.binanceService.GetTickers().subscribe(x => {
      this.tickers = x;
    });
  }
}
