import { Component, OnDestroy, OnInit,ViewChild } from '@angular/core';
import { BackendService } from '../services/backend.service';
import { MatTableDataSource,MatTable } from '@angular/material/table';
import { BehaviorSubject, Subscriber, Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../model/user.model';
import { Product } from '../model/product.model';
import { Router } from '@angular/router';
import {AccountInfo} from '@azure/msal-browser'

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
  constructor(
    private backendService: BackendService, 
    private authService: AuthService,
    private router: Router
  ){}
  isUserAdmin: boolean = false
  ngOnInit(){
     this.backendService.getAllProducts().subscribe({
       next: (products) => { this.dataSource = products},
       error: (error) => { console.log(`error: ${error.message}`)},
       complete: () => {}
     })

     this.userSubscription = this.authService.getCurrentUserListener()
     this.userSubscription.subscribe(user => {
       this.currentUser = user
       this.isUserAdmin =  this.authService.isUserAdmin()
     })
  }

  addProductMock(){
    let product: Product = 
      {
        id : 123,
        name : "asfd",
        description : "sdasdf",
        quantity : 21,
        price : 123123,
        category : "carr"
      }
    this.dataSource.push(product)
    this.myTable?.renderRows()
  }

  addProduct(){
    this.router.navigate(['createproduct'])
  }

  addCategory(){
    this.router.navigate(['createcategory'])
  }


  addToCart(product: Product){
    this.backendService.AddProductToBakset(1, product.id)
      .subscribe(result => {
        console.log('added: ' + product.id)
      })
  }

  editProduct(product: Product){
    console.log('id:' + product.id)
    this.router.navigate(['edit', product.id])
  }

  deleteProduct(product: Product){
    this.backendService.deleteProduct(product)
      .subscribe(_ =>      
        this.backendService.getAllProducts().subscribe({
        next: (products) => { this.dataSource = products},
        error: (error) => { console.log(`error: ${error.message}`)},
        complete: () => {}
      }))
  }

}
