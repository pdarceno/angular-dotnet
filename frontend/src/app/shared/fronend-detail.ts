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

  getOrderDetails(page: number, pageSize: number, sortDir: string = 'desc') {
    return this.http.get<Order[]>(
      `${this.url}/OrderDetails?page=${page}&pageSize=${pageSize}&sortDir=${sortDir}`
    );
  }

  searchOrders(query: string, page: number, pageSize: number) {
    return this.http.get<Order[]>(
      `${this.url}/OrderDetails/search?pizzaName=${query}&page=${page}&pageSize=${pageSize}`
    );
  }

  getPizzaDetails(
    page: number = 1,
    pageSize: number = 10,
    sortDir: string = 'desc'
  ) {
    return this.http.get<Pizza[]>(
      `${this.url}/Pizzas?page=${page}&pageSize=${pageSize}&sortDir=${sortDir}`
    );
  }

  searchPizzas(
    pizzaTypeName: string,
    category: string,
    size: string,
    minPrice?: number,
    maxPrice?: number,
    page: number = 1,
    pageSize: number = 10
  ) {
    let params = new URLSearchParams();
    if (pizzaTypeName) params.append('pizzaTypeName', pizzaTypeName);
    if (category) params.append('category', category);
    if (size) params.append('size', size);
    if (minPrice !== undefined) params.append('minPrice', minPrice.toString());
    if (maxPrice !== undefined) params.append('maxPrice', maxPrice.toString());
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());

    return this.http.get<Pizza[]>(
      `${this.url}/Pizzas/search?${params.toString()}`
    );
  }
}
