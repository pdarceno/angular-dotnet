import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class FrontendDetailService {
  url: string = environment.apiBaseUrl + '/OrderDetails';
  constructor(private http: HttpClient) {}

  getDetails() {
    this.http.get(this.url).subscribe({
      next: (data) => {
        console.log('Data received:', data);
      },
      error: (error) => {
        console.error('Error fetching data:', error);
      },
    });
  }
}
