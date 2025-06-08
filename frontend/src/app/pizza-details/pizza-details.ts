import { Component, OnInit } from '@angular/core';
import { FrontendDetailService } from '../shared/fronend-detail';
import { Pizza } from '../shared/frontend-detail.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pizza-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pizza-details.html',
})
export class PizzaDetailsComponent implements OnInit {
  pizzas: Pizza[] = [];
  selectedPizza?: Pizza;
  searchQuery: string = '';
  searchField: string = 'pizzaTypeName';
  currentPage = 1;
  pageSize = 10;

  constructor(private service: FrontendDetailService) {}

  ngOnInit(): void {
    this.loadPizzas();
  }

  loadPizzas(): void {
    this.service.getPizzaDetails(this.currentPage, this.pageSize).subscribe({
      next: (data) => (this.pizzas = data),
      error: (err) => console.error('Failed to load pizzas:', err),
    });
  }

  searchPizzas(): void {
    const name = this.searchField === 'pizzaTypeName' ? this.searchQuery : '';
    const category = this.searchField === 'category' ? this.searchQuery : '';
    const size = this.searchField === 'size' ? this.searchQuery : '';

    this.service
      .searchPizzas(
        name,
        category,
        size,
        undefined,
        undefined,
        this.currentPage,
        this.pageSize
      )
      .subscribe({
        next: (data) => (this.pizzas = data),
        error: (err) => console.error(err),
      });
  }

  goToNextPage(): void {
    this.currentPage++;
    this.searchQuery ? this.searchPizzas() : this.loadPizzas();
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.searchQuery ? this.searchPizzas() : this.loadPizzas();
    }
  }
}
