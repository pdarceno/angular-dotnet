import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/fronend-detail';

@Component({
  selector: 'app-order-details',
  imports: [],
  templateUrl: './order-details.html',
  styles: ``,
})
export class OrderDetailsComponent implements OnInit {
  constructor(public service: FrontendDetailService) {}
  ngOnInit() {
    this.service.getOrderDetails();
  }
}
