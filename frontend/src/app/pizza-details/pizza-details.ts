import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/fronend-detail';
@Component({
  selector: 'app-pizza-details',
  imports: [],
  templateUrl: './pizza-details.html',
  styles: ``,
})
export class PizzaDetialsComponent implements OnInit {
  constructor(public service: FrontendDetailService) {}
  ngOnInit(): void {
    this.service.getPizzaDetails();
  }
}
