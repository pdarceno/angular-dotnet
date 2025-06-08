import { Component } from '@angular/core';
import { OrderDetailsComponent } from './order-details/order-details';
import { PizzaDetailsComponent } from './pizza-details/pizza-details';
import { StatsDetailsComponent } from './stats-details/stats-details';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    OrderDetailsComponent,
    PizzaDetailsComponent,
    StatsDetailsComponent,
  ],
  templateUrl: './app.html',
})
export class App {}
