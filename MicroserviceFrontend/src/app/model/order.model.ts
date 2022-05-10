import { BasketProduct } from "./basket-product"

export interface OrderItem{
    orderId: string,
    orderDate: string
    totalCost: number
    products: BasketProduct[]
}

export interface Order{
    id: string,
    orderItems: OrderItem[],
    username: string,
    email: string,
    userId: string
}