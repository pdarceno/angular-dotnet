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
  public bestsellersOptions: Partial<ChartOptions> = {};
  public peakHoursOptions: Partial<ChartOptions> = {};
  public dailyOrdersOptions: Partial<ChartOptions> = {};
  public underperformersOptions: Partial<ChartOptions> = {};

  constructor(private statsService: StatisticsService) {}

  ngOnInit() {
    this.statsService.getBestsellers().subscribe((data) => {
      this.bestsellersOptions = {
        series: [{ name: 'Total Sold', data: data.map((p) => p.totalSold) }],
        chart: { height: 350, type: 'bar' },
        xaxis: {
          categories: data.map((p) => p.pizzaTypeName + ' (' + p.size + ')'),
        },
        title: { text: 'Top 5 Bestselling Pizzas', align: 'center' },
        dataLabels: { enabled: true },
      };
    });

    this.statsService.getPeakHours().subscribe((data) => {
      this.peakHoursOptions = {
        series: [{ name: 'Orders', data: data.map((p) => p.orderCount) }],
        chart: { height: 350, type: 'bar' },
        xaxis: { categories: data.map((p) => `${p.hour}:00`) },
        title: { text: 'Peak Ordering Hours', align: 'center' },
        dataLabels: { enabled: true },
      };
    });

    this.statsService.getDailyOrders().subscribe((data) => {
      this.dailyOrdersOptions = {
        series: [{ name: 'Orders', data: data.map((p) => p.orders) }],
        chart: { height: 350, type: 'line' },
        xaxis: { categories: data.map((p) => p.date) },
        title: { text: 'Daily Orders Over Time', align: 'center' },
        dataLabels: { enabled: false },
      };
    });

    this.statsService.getUnderperformers().subscribe((data) => {
      this.underperformersOptions = {
        series: [{ name: 'Not Ordered', data: data.map((p) => 1) }], // just count each item
        chart: { height: 350, type: 'bar' },
        xaxis: {
          categories: data.map((p) => p.pizzaTypeName + ' (' + p.size + ')'),
        },
        title: { text: 'Pizzas Never Ordered', align: 'center' },
        dataLabels: { enabled: false },
      };
    });
  }
}
