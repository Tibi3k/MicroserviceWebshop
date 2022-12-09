import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  IPayPalConfig,
  ICreateOrderRequest 
} from 'ngx-paypal';
import { BasketProduct } from 'src/app/model/basket-product';
import { Basket } from 'src/app/model/basket.model';
import { PaymentService } from 'src/app/services/payment.service';
@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  constructor(private paymentService: PaymentService, private router: Router){}

  public payPalConfig?: IPayPalConfig;
  public showSuccess: boolean = false
  public showCancel: boolean = false
  public showError: boolean = false

  @Input()
  public basket: Basket | null = null
  
  ngOnInit(): void {
      this.initConfig();
  }

  private initConfig(): void {
      this.payPalConfig = {
          clientId: 'AeMQteoytQtWWUhyNrO7LBHx0_-nrfQCKlS6Gs4VeobE8ZKc8je6YYGMqDcaVdK-W4czAP38fZZmnX4k',
          currency: 'HUF',
          // for creating orders (transactions) on server see
          // https://developer.paypal.com/docs/checkout/reference/server-integration/set-up-transaction/
          createOrderOnServer: (data) => this.paymentService.createOrder(this.basket!).toPromise().then(data => {
            console.log("promise", data)  
            if(data == undefined)
              return ""
            return data;
          }),
          onApprove: (data, actions) => {
              console.log('onApprove - transaction was approved, but not authorized', data, actions);
              actions.order.get().then((details: any) => {
                  console.log('onApprove - you can get full order details inside onApprove: ', details);
              });

          },
          onClientAuthorization: (data) => {
            console.log('transaction was authorized', data);
            
            this.paymentService.captureOrder(data).subscribe(result => {
              this.showSuccess = true;
              console.log('capture complete')
              this.paymentService
              this.paymentService.completeBasket(this.basket!).subscribe(result2 => {
                this.router.navigate(['/'])
              })
            })
            
          },
          // authorizeOnServer: (data, actions) => {
          //   return this.paymentService.captureOrder(data).toPromise().then(result => {
          //     this.showSuccess = true;
          //     console.log('data', data)
          //      console.log('capture complete')
          //    })
          // },
          onCancel: (data, actions) => {
              console.log('OnCancel', data, actions);
              this.showCancel = true;

          },
          onError: err => {
              console.log('OnError', err);
              this.showError = true;
          },
          onClick: (data, actions) => {
              console.log('onClick', data, actions);
              this.resetStatus();
          },
      };
  }
  
  resetStatus(){
    this.showCancel = false;
    this.showError = false;
    this.showSuccess = false;
  }


}
