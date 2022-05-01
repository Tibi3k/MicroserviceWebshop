import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class BackendService {
  API_URL: string = environment.apiUrl

  constructor(private http: HttpClient) {}

  private handleError (error: any) {
    const errMsg = (error.message) ? error.message :
      error.status ? `${error.status} - ${error.statusText}` : 'Server error';
    console.error(errMsg);
    return throwError(() => new Error(errMsg));
  }

   public getAllProducts(): Observable<Array<Product>>{
     return this.http
      .get<Array<Product>>(this.API_URL + 'api/products')
      .pipe(
        catchError(this.handleError)
      )
   }

   public getProductById(id: Number): Observable<Product>{
      return this.http
        .get<Product>(this.API_URL + `api/product/${id}`)
        .pipe(
          catchError(this.handleError)
        )
   }

   public createProduct(product: Product): Observable<string>{
     const body = JSON.stringify(product);
     return this.http
       .post<string>(this.API_URL + 'api/products', body).pipe(
         catchError(this.handleError)
       )
   }

   public deleteProduct(product: Product): Observable<string>{
    return this.http
      .delete<string>(this.API_URL + `api/products/${product.Id}`)
      .pipe(
        catchError(this.handleError)
      )
  }

  public AddProductToBakset(amount: Number, userId: Number, productID: Number){
      return this.http
        .post(this.API_URL + `api/products/${productID}/tobasket?amount=${amount}&userID=${userId}`, {})
        .pipe(
          catchError(this.handleError)
        )
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