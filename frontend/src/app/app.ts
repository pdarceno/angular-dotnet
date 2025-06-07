import { Component } from '@angular/core';
import { OrderDetails } from './order-details/order-details';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OrderDetails],
  templateUrl: './app.html',
})
export class App {}
