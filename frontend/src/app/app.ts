import { Component } from '@angular/core';
import { OrderDetailsComponent } from './order-details/order-details';
import { PizzaDetialsComponent } from './pizza-details/pizza-details';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OrderDetailsComponent, PizzaDetialsComponent],
  templateUrl: './app.html',
})
export class App {}
