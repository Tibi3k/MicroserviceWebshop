import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { BehaviorSubject } from 'rxjs';
import { BasketProduct } from '../../model/basket-product';
import { Basket } from '../../model/basket.model';
import { Product } from '../../model/product.model';
import { RequestState } from '../../model/request-state.model';
import { BasketService } from '../../services/basket.service';

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
  errorMap: Array<Number> = []
  loadingMap: Array<Number> = []
  orderLoading = false
  orderError = false

  constructor(
    private basketService: BasketService, 
  ){}


  ngOnInit(): void {
    this.getUserBasket()
  }

  removeItemFromCart(product: BasketProduct){
    this.removeItemFromList(this.errorMap, product.id)
    this.loadingMap.push(product.id)  
    this.basketService.deleteProductFromBasket(product.basketSubId)
      .subscribe({
        next:(result) => {
          this.removeItemFromList(this.loadingMap, product.id)
          this.basketService.getUserBasket()
        },
        error: (error) => {
          this.removeItemFromList(this.loadingMap, product.id)
          this.errorMap.push(product.id)
        }
    })
  }

  orderBasket(){
    this.orderLoading = true
    this.basketService.orderBasket()
      .subscribe({
        next: result => {
          this.orderLoading = false
          this.getUserBasket()
        },
        error: error => {
          this.orderLoading = false
          this.orderError = true
        }
      })
  }

  getUserBasket(){
    this.loadState = this.state.loading
    this.basketService.getUserBasket()
      .subscribe({
        next:(basket) => {
          this.basket = basket
          if(basket != null){
            this.dataSource = basket.products
          } else {
            this.dataSource = []
          }
          this.loadState = this.state.success
        },
        error: (error) => {
          this.loadState = this.state.error
          this.errorMsg = error?.message
        }
    })
  }

  removeItemFromList(list: Array<Number>, id: Number){
    let index = list.indexOf(id)
    if(index > -1)
      list.splice(index, 1)
  }

}

