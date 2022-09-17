import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { NgChartsModule } from 'ng2-charts';



@NgModule({
  exports:[
    DashboardComponent
  ],
  declarations: [
    DashboardComponent
  ],
  imports: [
    CommonModule,
    NgChartsModule
  ]
})
export class BinanceModule { }
