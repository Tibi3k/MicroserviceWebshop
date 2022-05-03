import { Component, OnInit } from '@angular/core';
import { Product } from './model/product.model';
import { BackendService } from './services/backend.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'MicroserviceFrontend';

  constructor(private backendService: BackendService){}
  ngOnInit(){
    this.backendService.getAllProducts().subscribe((products: Array<Product>) => console.log(products))
  }
}
