import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { BehaviorSubject } from 'rxjs';
import { BasketProduct } from '../model/basket-product';
import { Basket } from '../model/basket.model';
import { Product } from '../model/product.model';
import { RequestState } from '../model/request-state.model';
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
  loadState = RequestState.loading
  state = RequestState
  errorMsg = "Something went wrong, please try again later!"

  constructor(
    private basketService: BasketService, 
  ){}


  ngOnInit(): void {
    this.getUserBasket()
  }

  removeItemFromCart(product: BasketProduct){
    this.loadState = this.state.loading
    this.basketService.deleteProductFromBasket(product.basketSubId)
      .subscribe({
        next:(result) => {
          this.basketService.getUserBasket()
          this.loadState = RequestState.success
        },
        error: (error) => {
          this.loadState = this.state.error
          this.errorMsg = error?.message
        }
    })
  }

  orderBasket(){
    this.basketService.orderBasket()
      .subscribe(result => {this.getUserBasket()})
  }

  getUserBasket(){
    this.loadState = this.state.loading
    this.basketService.getUserBasket()
      .subscribe({
        next:(basket) => {
          this.basket = basket
          if(basket != null){
            this.dataSource = basket.products
            this.myTable?.renderRows()
          }
          this.loadState = this.state.success
        },
        error: (error) => {
          this.loadState = this.state.error
          this.errorMsg = error?.message
        }
    })
  }
}

