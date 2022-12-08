import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { IClientAuthorizeCallbackData, IOnApproveCallbackData } from "ngx-paypal";
import { map, Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Basket } from "../model/basket.model";
import { CreatePaypalOrder } from "../model/create-order";

@Injectable({
    providedIn: 'root'
  })
  export class PaymentService {
    API_URL: string = environment.apiUrl
    
    constructor(private http: HttpClient) {}

    createOrder(basket: Basket): Observable<string> {
      return this.http.get<CreatePaypalOrder>("api/payment")
        .pipe(map((response: CreatePaypalOrder) => {
          console.log(response)
          console.log(response.id)
          return response.id
        }))
    }

    captureOrder(body: IClientAuthorizeCallbackData): Observable<string> {
      return this.http.post<string>("api/payment", body)
    }

    completeBasket(basket: Basket): Observable<string> {
      return this.http.post<string>("api/payment/complete", basket)
  }
}