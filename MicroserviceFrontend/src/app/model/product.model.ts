export interface Product {
    id: Number;
    name: String;
    description: String;
    price: Number
    quantity: Number
    category: String
  }

export interface ProductCreate{
    name: String;
    description: String;
    price: Number
    quantity: Number
    category: String
}