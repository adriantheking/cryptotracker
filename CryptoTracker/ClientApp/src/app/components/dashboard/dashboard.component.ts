import { Component, OnInit } from "@angular/core";
import { DashboardService, ZondaTransactionHistoryModel } from "src/app/services/dashboard.service";
import { ZondaService } from "src/app/services/zonda.service";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
  })
  export class DashboardComponent implements OnInit {
    public dashboardItems?: ZondaTransactionHistoryModel;
    public zondaInvestedAmount: number = 0;

    constructor(private dashboardService: DashboardService,
      private zondaService: ZondaService){

    }

    ngOnInit(): void {
        this.getDashboard();
        this.getZondaInvestedAmount();
    }


    public getDashboard(){
        this.dashboardService.GetDashboard().subscribe(x => this.dashboardItems = x);
    }

    public getZondaInvestedAmount(){
      this.zondaService.GetInvestedAmount().subscribe(x => this.zondaInvestedAmount = x);
    }
  }