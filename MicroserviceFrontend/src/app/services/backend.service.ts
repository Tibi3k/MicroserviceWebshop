import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Product, ProductCreate } from '../model/product.model';
import { Category } from '../model/category.model';


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
      .get<Array<Product>>(this.API_URL + 'api/products/public')
      .pipe(
        catchError(this.handleError)
      )
   }

   public getProductById(id: Number): Observable<Product>{
      return this.http
        .get<Product>(this.API_URL + `api/products/public/${id}`)
        .pipe(
          catchError(this.handleError)
        )
   }

   public createProduct(product: ProductCreate): Observable<string>{
     const body = JSON.stringify(product);
     return this.http
       .post<string>(
           this.API_URL + 'api/products/protected', 
           body, 
           {headers: this.getHeaders()
        }).pipe(
         catchError(this.handleError)
       )
   }

   public editProduct(product: Product): Observable<string>{
    const body = JSON.stringify(product);
    return this.http
      .put<string>(this.API_URL + 'api/products/protected', body, {headers: this.getHeaders()}).pipe(
        catchError(this.handleError)
      )
  }

   public deleteProduct(product: Product): Observable<string>{
    return this.http
      .delete<string>(this.API_URL + `api/products/protected/${product.id}/`)
      .pipe(
        catchError(this.handleError)
      )
  }

  public AddProductToBakset(amount: Number, productID: Number){
      return this.http
        .post(
          this.API_URL + `api/products/protected/${productID}/tobasket?amount=${amount}`,
           {},
           {headers: this.getHeaders()}
        )
        .pipe(
          catchError(this.handleError)
        )
  }

  public AddCategory(name: string): Observable<Category>{
    const body = JSON.stringify(name);
    return this.http
      .post<Category>(
        this.API_URL + 'api/categories/create',
         body,
         {headers: this.getHeaders() }
        ).pipe(
        catchError(this.handleError)
      )
  }

  public getAllCategories(): Observable<Array<Category>>{
    return this.http
      .get<Array<Category>>(this.API_URL + 'api/categories/get')
      .pipe(
        catchError(this.handleError)
      )

  }

  getHeaders(): HttpHeaders{
    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'application/json; charset=utf-8');
    return headers
  }
}

