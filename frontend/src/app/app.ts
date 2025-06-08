import { Component } from '@angular/core';
import { OrderDetailsComponent } from './order-details/order-details';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OrderDetailsComponent],
  templateUrl: './app.html',
})
export class App {}
