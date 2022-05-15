import { Component, OnDestroy, OnInit,ViewChild } from '@angular/core';
import { BackendService } from '../services/backend.service';
import { MatTableDataSource,MatTable } from '@angular/material/table';
import { BehaviorSubject, Subscriber, Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../model/user.model';
import { Product } from '../model/product.model';
import { Router } from '@angular/router';
import {AccountInfo} from '@azure/msal-browser'
import { RequestState } from '../model/request-state.model';
import { ErrorObject } from '../model/errors.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category', 'actions'];
  dataSource: Array<Product> = [];
  currentUser: AccountInfo | null = null
  @ViewChild(MatTable) myTable: MatTable<any> | undefined;
  userSubscription!: BehaviorSubject<AccountInfo | null>

  state = RequestState
  loadState = RequestState.loading
  isUserAdmin: boolean = false
  errorMsg = 'Something went wrong, please try again later!!'
  loadingMap: Array<Number> = []
  errorMap: Array<Number> = []
  deleteLoadingMap: Array<Number> = []
  deleteErrorMap: Array<Number> = []

  constructor(
    private backendService: BackendService, 
    private authService: AuthService,
    private router: Router
  ){}

  ngOnInit(){
    this.getAvailableProducts()

     this.userSubscription = this.authService.getCurrentUserListener()
     this.userSubscription.subscribe(user => {
       this.currentUser = user
       this.isUserAdmin =  this.authService.isUserAdmin()
     })
  }

  getAvailableProducts(){
    this.loadState = this.state.loading
     this.backendService.getAllProducts().subscribe({
       next: (products) => { 
         this.loadState = this.state.success
         this.dataSource = products
        },
       error: (error: string) => {
         console.log('error', error)
          this.errorMsg = error
          this.loadState = this.state.error
        }
      })
  }

  addToCart(product: Product){
    this.loadingMap.push(product.id)
    this.removeItemFromList(this.errorMap, product.id)
    this.backendService.AddProductToBakset(1, product.id)
      .subscribe({
        next: result => {
        console.log('added: ' + product.id)
        this.removeItemFromList(this.loadingMap, product.id)
        },
        error: (error: string )=> {
          console.log('plc:', error)
          this.errorMsg = error
          this.removeItemFromList(this.loadingMap, product.id)
          this.errorMap.push(product.id)
        }
      })
  }

  deleteProduct(product: Product){
    this.deleteLoadingMap.push(product.id)
    this.removeItemFromList(this.deleteErrorMap, product.id)
    this.backendService.deleteProduct(product)
      .subscribe({
        next: _ => {
          this.removeItemFromList(this.deleteLoadingMap, product.id)
          this.getAvailableProducts()
        },
        error: error => {
          this.errorMsg = error
          this.removeItemFromList(this.deleteLoadingMap, product.id)
          this.deleteErrorMap.push(product.id)
        }
    })
  }

  removeItemFromList(list: Array<Number>, id: Number){
    let index = list.indexOf(id)
    if(index > -1)
      list.splice(index, 1)
  }

}
