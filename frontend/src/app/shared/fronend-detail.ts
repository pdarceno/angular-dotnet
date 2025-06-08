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

  getOrderDetails(page: number, pageSize: number) {
    return this.http.get<Order[]>(
      `${this.url}/OrderDetails?page=${page}&pageSize=${pageSize}`
    );
  }

  searchOrders(query: string, page: number, pageSize: number) {
    return this.http.get<Order[]>(
      `${this.url}/OrderDetails/search?pizzaName=${query}&page=${page}&pageSize=${pageSize}`
    );
  }

  getOrderDetailById(id: number) {
    return this.http.get<Order>(`${this.url}/OrderDetails/${id}`);
  }

  getPizzaDetails(page: number = 1, pageSize: number = 10) {
    return this.http.get<Pizza[]>(
      `${this.url}/Pizzas?page=${page}&pageSize=${pageSize}`
    );
  }
}
