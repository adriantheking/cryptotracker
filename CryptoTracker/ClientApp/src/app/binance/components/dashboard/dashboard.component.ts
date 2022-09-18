import { Component, OnInit, ViewChild } from '@angular/core';
import { ChartConfiguration, Chart, ChartData, ChartEvent, ChartType } from 'chart.js';
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
    labels: [],
    datasets: [{
      data: []
    }]
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [DatalabelsPlugin];
  //line
  public lineChartOptions: ChartConfiguration['options'] = {
    elements: {
      line: {
        tension: 0.1
      }
    },
    scales: {
      // We use this empty structure as a placeholder for dynamic theming.
      x: {},
      'y-axis-0':
        {
          position: 'left',
        },
      'y-axis-1': {
        position: 'right',
        grid: {
          color: 'rgba(255,0,0,0.3)',
        },
        ticks: {
          color: 'red'
        }
      }
    },

    plugins: {
      legend: { display: true },
    }
  };
  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [ 65, 59, 80, 81, 56, 55, 40 ],
        label: 'Series A',
        backgroundColor: 'rgba(148,159,177,0.2)',
        borderColor: 'rgba(148,159,177,1)',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        fill: 'origin',
      },
      {
        data: [ 28, 48, 40, 19, 86, 27, 90 ],
        label: 'Series B',
        backgroundColor: 'rgba(77,83,96,0.2)',
        borderColor: 'rgba(77,83,96,1)',
        pointBackgroundColor: 'rgba(77,83,96,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(77,83,96,1)',
        fill: 'origin',
      },
      {
        data: [ 180, 480, 770, 90, 1000, 270, 400 ],
        label: 'Series C',
        yAxisID: 'y-axis-1',
        backgroundColor: 'rgba(255,0,0,0.3)',
        borderColor: 'red',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        fill: 'origin',
      }
    ],
    labels: [ 'January', 'February', 'March', 'April', 'May', 'June', 'July' ]
  };
  public lineChartType: ChartType = 'line';
  // events
  public chartClicked({ event, active }: { event: ChartEvent, active: {}[] }): void {
    console.log(event, active);
  }

  public chartHovered({ event, active }: { event: ChartEvent, active: {}[] }): void {
    console.log(event, active);
  }
  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.getTickers();
    this.getWallet();

  }

  private getWallet() {
    this.binanceService.wallet$.subscribe(x => {
      this.wallet = x;
      this.preparePieChartWithCoinsWeight();
    })
  }

  private getTickers() {
    this.binanceService.tickers$.subscribe(tickers => {
      this.tickers = tickers;
      this.preparePieChartWithCoinsWeight();
    })
  }

  private preparePieChartWithCoinsWeight() {
    this.binanceService.CalculateCoinsWeight().then(coinsInfo => {
      this.pieChartData = {
        labels: [],
        datasets: [{
          data: []
        }]
      };

      for (var i = 0; i < coinsInfo.length; i++) {
        this.pieChartData.labels?.push(coinsInfo[i].symbol!);
        this.pieChartData.datasets[0].data.push(Math.round(coinsInfo[i].weight! * 100) / 100);
      }
    });
  }
}
