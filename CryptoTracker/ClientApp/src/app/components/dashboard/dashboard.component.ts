import { Component, OnInit } from "@angular/core";
import { ZondaBalanceModel } from "src/app/models/zonda/ZondaBalances.model";
import { DashboardService, ZondaTransactionHistoryModel } from "src/app/services/dashboard.service";
import { ZondaService } from "src/app/services/zonda.service";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
  })
  export class DashboardComponent implements OnInit {
    public dashboardItems?: ZondaTransactionHistoryModel;
    public zondaInvestedAmount: number = 0;
    public zondaWallets: ZondaBalanceModel[] = [];

    constructor(private dashboardService: DashboardService,
      private zondaService: ZondaService){

    }

    ngOnInit(): void {
        this.getDashboard();
        this.getZondaInvestedAmount();
        this.getZondaWallets();
    }


    public getDashboard(){
        this.dashboardService.GetDashboard().subscribe(x => this.dashboardItems = x);
    }

    public getZondaInvestedAmount(){
      this.zondaService.GetInvestedAmount().subscribe(x => this.zondaInvestedAmount = x);
    }
    public getZondaWallets(){
      this.zondaService.GetWallets().subscribe(x => this.zondaWallets = x);
    }
  }