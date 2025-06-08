import { Component } from '@angular/core';
import { OrderDetailsComponent } from './order-details/order-details';
import { PizzaDetailsComponent } from './pizza-details/pizza-details';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OrderDetailsComponent, PizzaDetailsComponent],
  templateUrl: './app.html',
})
export class App {}
