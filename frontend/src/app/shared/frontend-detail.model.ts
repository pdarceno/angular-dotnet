import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class FrontendDetailService {
  url: string = environment.apiBaseUrl;
  constructor(private http: HttpClient) {}

  getOrderDetails() {
    this.http.get(this.url + '/OrderDetails').subscribe({
      next: (data) => {
        console.log('Data received:', data);
      },
      error: (error) => {
        console.error('Error fetching data:', error);
      },
    });
  }

  getPizzaDetails() {
    this.http.get(this.url + '/Pizzas').subscribe({
      next: (data) => {
        console.log('Data received:', data);
      },
      error: (error) => {
        console.error('Error fetching data:', error);
      },
    });
  }
}
