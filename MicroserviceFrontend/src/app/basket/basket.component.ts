import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { BehaviorSubject } from 'rxjs';
import { BasketProduct } from '../model/basket-product';
import { Basket } from '../model/basket.model';
import { Product } from '../model/product.model';
import { BasketService } from '../services/basket.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})
export class BasketComponent implements OnInit {

  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category','lastEdited','actions'];
  dataSource: Array<BasketProduct> = [];
  basket: Basket | null = null
  @ViewChild(MatTable) myTable: MatTable<any> | undefined;
  constructor(
    private basketService: BasketService, 
  ){}


  ngOnInit(): void {
    this.getUserBasket()
  }

  removeItemFromCart(product: BasketProduct){
    this.basketService.deleteProductFromBasket(product.basketSubId)
      .subscribe(result => {
        this.basketService.getUserBasket()
      })
  }

  orderBasket(){
    this.basketService.orderBasket()
      .subscribe(result => {})
  }

  getUserBasket(){
    this.basketService.getUserBasket()
      .subscribe(basket => {
        this.basket = basket
        this.dataSource = basket.products
        this.myTable?.renderRows()
      })
  }
}
