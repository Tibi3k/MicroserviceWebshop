import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Order } from '../model/order.model';


@Injectable({
  providedIn: 'root'
})
export class OrderService {
  API_URL: string = environment.apiUrl

  constructor(private http: HttpClient) {}

  public getOrdersOfUser(){
      return this.http.get<Order>(this.API_URL + 'api/orders')
  }
}
