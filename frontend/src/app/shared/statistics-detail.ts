import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  constructor(private http: HttpClient) {}

  getBestsellers() {
    return this.http.get<any[]>('/api/statistics/bestsellers');
  }

  getPeakHours() {
    return this.http.get<any[]>('/api/statistics/peak-hours');
  }

  getDailyOrders() {
    return this.http.get<any[]>('/api/statistics/daily-orders');
  }

  getUnderperformers() {
    return this.http.get<any[]>('/api/statistics/underperformers');
  }
}
