import { Component, OnDestroy, OnInit,ViewChild } from '@angular/core';
import { BackendService } from '../services/backend.service';
import { MatTableDataSource,MatTable } from '@angular/material/table';
import { BehaviorSubject, Subscriber, Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../model/user.model';
import { Product } from '../model/product.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category', 'actions'];
  dataSource: Array<Product> = [];
  currentUser: User | null = null
  @ViewChild(MatTable) myTable: MatTable<any> | undefined;
  userSubscription!: BehaviorSubject<User | null>
  constructor(
    private backendService: BackendService, 
    private authService: AuthService,
    private router: Router
  ){}

  ngOnInit(){
     this.backendService.getAllProducts().subscribe({
       next: (products) => { this.dataSource = products},
       error: (error) => { console.log(`error: ${error.message}`)},
       complete: () => {}
     })

     this.userSubscription = this.authService.getCurrentUserListener()
     this.userSubscription.subscribe(user => {
       this.currentUser = user
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

  // ngOnDestroy(){
  //   this.userSubscription?.unsubscribe()
  // }

  addToCart(product: Product){
    console.log('id:' + product.id)
  }

  editProduct(product: Product){
    console.log('id:' + product.id)
    this.router.navigate(['edit', product.id])
  }

}
