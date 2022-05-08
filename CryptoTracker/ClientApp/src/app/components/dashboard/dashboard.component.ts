import { Component, OnInit } from "@angular/core";
import { DashboardService, ZondaTransactionHistoryModel } from "src/app/services/dashboard.service";

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
  })
  export class DashboardComponent implements OnInit {
    public dashboardItems?: ZondaTransactionHistoryModel;

    constructor(private dashboardService: DashboardService){

    }

    ngOnInit(): void {
        this.GetDashboard();
    }


    public GetDashboard(){
        this.dashboardService.GetDashboard().subscribe(x => this.dashboardItems = x);
    }
  }