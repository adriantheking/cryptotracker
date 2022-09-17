import { Component, OnInit, ViewChild } from "@angular/core";
import { ChartData, ChartType } from "chart.js";
import { BaseChartDirective } from "ng2-charts";
import { BinanceTickersModel } from "src/app/binance/models/binanceTickers.model";
import { IWallet } from "src/app/models/wallet.model";
import { ZondaCryptoBalanceModel } from "src/app/models/zonda/zondaCryptoBalance.model";
import { BinanceService } from "src/app/services/binance.service";
import { DashboardService, ZondaTransactionHistoryModel } from "src/app/services/dashboard.service";
import { ZondaService } from "src/app/services/zonda.service";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
  })
  export class DashboardComponent implements OnInit {
    @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
    public wallet: IWallet = {};
    public tickers: BinanceTickersModel = {};
    public dashboardItems?: ZondaTransactionHistoryModel;
    public zondaInvestedAmount: number = 0;
    public zondaCryptoBalances: ZondaCryptoBalanceModel[] = [];
//charts
    public pieChartType: ChartType = 'pie';
    public pieChartData: ChartData<'pie', number[], string | string[]> = {
      labels: [ ],
      datasets: []
    };
    constructor(private dashboardService: DashboardService,
      private binanceService: BinanceService){

    }

    ngOnInit(): void {
        // this.getDashboard();
        // this.getZondaInvestedAmount();
        // this.getZondaCryptoBalance();
    }




    // public getDashboard(){
    //     this.dashboardService.GetDashboard().subscribe(x => this.dashboardItems = x);
    // }

    // public getZondaInvestedAmount(){
    //   this.zondaService.GetInvestedAmount().subscribe(x => this.zondaInvestedAmount = x);
    // }
    // public getZondaCryptoBalance(){
    //   this.zondaService.GetCryptoBalance().subscribe(x => {
    //     this.zondaCryptoBalances = x;
    //     this.prepareCurrentWeightPie(x);
    //   });
    // }
    // private prepareCurrentWeightPie(balance:ZondaCryptoBalanceModel[]){
    //   let values:number[]= balance.map(x => {
    //     return Math.ceil(<number>x.worth);
    //   }) as number[];
    //   this.pieChartData.datasets.push({data: values});
    //   this.chart?.update();
    // }
  }