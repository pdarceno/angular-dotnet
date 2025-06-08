import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/frontend-detail.model';

@Component({
  selector: 'app-order-details',
  imports: [],
  templateUrl: './order-details.html',
  styles: ``,
})
export class OrderDetailsComponent implements OnInit {
  constructor(public service: FrontendDetailService) {}
  ngOnInit() {
    this.service.getDetails();
  }
}
