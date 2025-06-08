import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/fronend-detail';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Order } from '../shared/frontend-detail.model';

@Component({
  selector: 'app-order-details',
  imports: [CommonModule, FormsModule],
  templateUrl: './order-details.html',
  styles: ``,
})
export class OrderDetailsComponent implements OnInit {
  constructor(public service: FrontendDetailService) {}
  orders: Order[] = [];
  selectedOrder?: Order;
  searchQuery: string = '';
  currentPage = 1;
  pageSize = 5;

  ngOnInit() {
    this.loadOrders();
  }

  expandedOrderId?: number;

  toggleExpanded(orderId: number): void {
    this.expandedOrderId =
      this.expandedOrderId === orderId ? undefined : orderId;
  }

  loadOrders() {
    this.service.getOrderDetails(this.currentPage, this.pageSize).subscribe({
      next: (data) => (this.orders = data),
      error: (err) => console.error(err),
    });
  }

  searchOrders() {
    this.service
      .searchOrders(this.searchQuery, this.currentPage, this.pageSize)
      .subscribe({
        next: (data) => (this.orders = data),
        error: (err) => console.error(err),
      });
  }

  goToNextPage() {
    this.currentPage++;
    this.searchQuery ? this.searchOrders() : this.loadOrders();
  }

  goToPreviousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.searchQuery ? this.searchOrders() : this.loadOrders();
    }
  }
}
