import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Product, ProductCreate } from '../model/product.model';
import { Category } from '../model/category.model';
import { Basket } from '../model/basket.model';


@Injectable({
  providedIn: 'root'
})
export class BasketService {
  API_URL: string = environment.apiUrl
  private basketCountSubject: BehaviorSubject<number> = new BehaviorSubject<number>(0)

  constructor(private http: HttpClient) {}

  private handleError (error: any) {
    const errMsg = (error.message) ? error.message :
      error.status ? `${error.status} - ${error.statusText}` : 'Server error';
    console.error(errMsg);
    return throwError(() => new Error(errMsg));
  }

  public getUserBasket(): Observable<Basket>{
      return this.http.get<Basket>(this.API_URL + 'api/basket/userbasket')
      .pipe(catchError(this.handleError))
  }

  orderBasket(){
    return this.http.post(this.API_URL + 'api/basket/order', {})
  } 

  deleteProductFromBasket(id: string){
    return this.http.delete<string>(this.API_URL + `api/basket/${id}`)
  }

  getBasketSize(){
    return this.http.get<number>(this.API_URL + `api/basket/size`)
  }

  getBasketCountListener(){
    return this.basketCountSubject
  }

  getHeaders(): HttpHeaders{
    let headers = new HttpHeaders();
    headers = headers.set('Content-Type', 'application/json; charset=utf-8');
    return headers
  }




}