import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexTitleSubtitle,
} from 'ng-apexcharts';

import { StatisticsService } from '../shared/statistics-detail';
import { Component, OnInit } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  title: ApexTitleSubtitle;
  dataLabels: ApexDataLabels;
};

@Component({
  selector: 'app-stats-details',
  imports: [NgApexchartsModule],
  templateUrl: './stats-details.html',
  styles: ``,
})
export class StatsDetailsComponent implements OnInit {
  public chartOptions: Partial<ChartOptions> = {}; // 👈 initialize it

  constructor(private statsService: StatisticsService) {}

  ngOnInit() {
    this.statsService.getBestsellers().subscribe((data) => {
      this.chartOptions = {
        series: [
          {
            name: 'Total Sold',
            data: data.map((p) => p.totalSold),
          },
        ],
        chart: {
          height: 350,
          type: 'bar',
        },
        xaxis: {
          categories: data.map((p) => p.pizzaTypeName + ' (' + p.size + ')'),
        },
        title: {
          text: 'Top 5 Bestselling Pizzas',
          align: 'center',
        },
        dataLabels: {
          enabled: true,
        },
      };
    });
  }
}
