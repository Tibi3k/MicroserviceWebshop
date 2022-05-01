import { Component, OnInit } from '@angular/core';
import { BackendService, Product } from '../services/backend.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  displayedColumns: string[] = ['#', 'name', 'price', 'description', 'quantity', 'category'];
  dataSource: Array<Product> = []

  
  constructor(private backendService: BackendService){}
  ngOnInit(){
    let data: Array<Product> = []
    data.push({
      Id : 123,
      Name : "asfd",
      Description : "sdasdf",
      Quantity : 21,
      Price : 123123,
      Category : "carr"
    })
    data.push({
      Id : 123,
      Name : "asfd",
      Description : "sdasdf",
      Quantity : 21,
      Price : 123123,
      Category : "carr"
    })
    this.dataSource = data
    this.backendService.getAllProducts().subscribe({
      next: (products) => { this.dataSource = products},
      error: (error) => { console.log(`error: ${error.message}`)},
      complete: () => {this.dataSource.push({
        Id : 123,
        Name : "asfd",
        Description : "sdasdf",
        Quantity : 21,
        Price : 123123,
        Category : "carr"
      })}
    })
  }

}
