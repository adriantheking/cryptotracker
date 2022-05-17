import { Component, OnInit } from "@angular/core";
import { ZondaCryptoBalanceModel } from "src/app/models/zonda/zondaCryptoBalance.model";
import { DashboardService, ZondaTransactionHistoryModel } from "src/app/services/dashboard.service";
import { ZondaService } from "src/app/services/zonda.service";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
  })
  export class DashboardComponent implements OnInit {
    public dashboardItems?: ZondaTransactionHistoryModel;
    public zondaInvestedAmount: number = 0;
    public zondaCryptoBalances: ZondaCryptoBalanceModel[] = [];

    constructor(private dashboardService: DashboardService,
      private zondaService: ZondaService){

    }

    ngOnInit(): void {
        this.getDashboard();
        this.getZondaInvestedAmount();
        this.getZondaCryptoBalance();
    }


    public getDashboard(){
        this.dashboardService.GetDashboard().subscribe(x => this.dashboardItems = x);
    }

    public getZondaInvestedAmount(){
      this.zondaService.GetInvestedAmount().subscribe(x => this.zondaInvestedAmount = x);
    }
    public getZondaCryptoBalance(){
      this.zondaService.GetCryptoBalance().subscribe(x => this.zondaCryptoBalances = x);
    }
  }