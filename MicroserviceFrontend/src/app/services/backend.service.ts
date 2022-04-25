import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class BackendService {
  API_URL: string = environment.apiUrl

  constructor(private http: HttpClient) {
   }

   public getAllProducts(): Observable<Array<Product>>{
     return this.http.get<Array<Product>>(this.API_URL + 'api/products')
   }
}

export interface Product {
  Id: Number;
  Name: String;
  Description: String;
  Price: Number
  Quantity: Number
  Category: String
}