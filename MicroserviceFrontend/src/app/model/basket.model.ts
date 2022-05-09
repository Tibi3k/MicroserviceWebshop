import { BasketProduct } from "./basket-product"
import { Product } from "./product.model"

export interface Basket{
    id: string
    userId: string
    products: Array<BasketProduct>
    lastEdited: string
    totalCost: Number
}