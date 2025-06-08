import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  url: string = environment.apiBaseUrl;
  constructor(private http: HttpClient) {}

  getBestsellers() {
    return this.http.get<any[]>(this.url + '/Statistics/bestsellers');
  }

  getPeakHours() {
    return this.http.get<any[]>(this.url + '/Statistics/peak-hours');
  }

  getDailyOrders() {
    return this.http.get<any[]>(this.url + '/Statistics/daily-orders');
  }

  getUnderperformers() {
    return this.http.get<any[]>(this.url + '/Statistics/underperformers');
  }
}
