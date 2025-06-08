import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Order, Pizza } from './frontend-detail.model';

@Injectable({
  providedIn: 'root',
})
export class FrontendDetailService {
  url: string = environment.apiBaseUrl;
  constructor(private http: HttpClient) {}

  getOrderDetails() {
    return this.http.get<Order[]>(this.url + '/OrderDetails');
  }

  getPizzaDetails() {
    return this.http.get<Pizza[]>(this.url + '/Pizzas');
  }
}
