// mapped to backend's DTO
export interface Order {
  orderId: number;
  dateTime: string;
  totalPrice: number;
  pizzaCount: number;
  details: OrderDetail[];
}

export interface OrderDetail {
  orderDetailId: number;
  pizzaName: string;
  size: string;
  quantity: number;
  price: number;
}

export interface Pizza {
  pizzaId: number;
  pizzaTypeName: number;
  category: string;
  size: string;
  price: number;
  totalSold: number;
}

export interface PizzaType {
  pizzaTypeId: number;
  name: string;
  category: string;
  ingredients: string;
  pizzas: Pizza[];
}
