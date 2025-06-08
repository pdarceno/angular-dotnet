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
  currentPage = 1;
  pageSize = 20;

  loadOrders() {
    this.service.getOrderDetails(this.currentPage, this.pageSize).subscribe({
      next: (data) => {
        this.orders = data;
      },
      error: (err) => console.error('Failed to load orders:', err),
    });
  }

  ngOnInit(): void {
    this.loadOrders();
  }

  goToPreviousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadOrders();
    }
  }

  goToNextPage() {
    this.currentPage++;
    this.loadOrders();
  }
}
