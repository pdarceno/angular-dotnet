import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/fronend-detail';
import { CommonModule } from '@angular/common';
import { Order } from '../shared/frontend-detail.model';

@Component({
  selector: 'app-order-details',
  imports: [CommonModule],
  templateUrl: './order-details.html',
  styles: ``,
})
export class OrderDetailsComponent implements OnInit {
  constructor(public service: FrontendDetailService) {}
  orders: Order[] = [];

  ngOnInit(): void {
    this.service.getOrderDetails().subscribe({
      next: (data) => {
        this.orders = data;
        console.log('Order data:', data);
      },
      error: (error) => {
        console.error('Error:', error);
      },
    });
  }
}
