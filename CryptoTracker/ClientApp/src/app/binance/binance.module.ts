import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './components/dashboard/dashboard.component';



@NgModule({
  exports:[
    DashboardComponent
  ],
  declarations: [
    DashboardComponent
  ],
  imports: [
    CommonModule
  ]
})
export class BinanceModule { }
