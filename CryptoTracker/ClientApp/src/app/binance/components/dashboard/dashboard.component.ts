import { Component, OnInit, ViewChild } from '@angular/core';
import { ChartConfiguration, ChartData, ChartEvent, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { IWallet } from 'src/app/models/wallet.model';
import { BinanceTickersModel } from '../../models/binanceTickers.model';
import { BinanceService } from '../../services/binance.service';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';

@Component({
  selector: 'binance-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public wallet: IWallet = {};
  public tickers: BinanceTickersModel[] = [];
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;

  // Pie
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'top',
      },
      
    }
  };
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [ [ 'Download', 'Sales' ], [ 'In', 'Store', 'Sales' ], 'Mail Sales' ],
    datasets: [ {
      data: [ 300, 500, 100 ]
    } ]
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [ DatalabelsPlugin ];

  // events
  public chartClicked({ event, active }: { event: ChartEvent, active: {}[] }): void {
    console.log(event, active);
  }

  public chartHovered({ event, active }: { event: ChartEvent, active: {}[] }): void {
    console.log(event, active);
  }
  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.getWallet();
    this.getTickers();
    
  }

  private getWallet(){
    this.binanceService.GetWallet().subscribe(x => {
      this.wallet = x;
      this.preparePieChartWithCoinsWeight();
    });
  }

  private getTickers(){
    this.binanceService.GetTickers().subscribe(x => {
      this.tickers = x;
    });
  }

  private preparePieChartWithCoinsWeight(){
    this.binanceService.CalculateCoinsWeight().then(coinsInfo => {
      this.pieChartData = {
        labels: [],
        datasets: [ {
          data: [  ]
        } ]
      };

      for(var i=0; i<coinsInfo.length; i++){
        this.pieChartData.labels?.push(coinsInfo[i].symbol!);
        this.pieChartData.datasets[0].data.push(coinsInfo[i].weight!);  
      }
    });
  }
}
